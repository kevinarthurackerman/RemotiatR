using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RemotiatR.Shared;
using System.IO;
using RemotiatR.Client;
using System.Collections.Generic;
using System.Linq;

namespace RemotiatR.MessageTransport.Http.Client
{
    internal class DefaultHttpMessageTransport : IMessageTransport
    {
        private const string _messageLengthsHeader = "message-lengths";

        private static readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(50);

        private static HttpClient? _lastHttpClient = null;

        private static IList<BatchedMessage> _messageBuffer = new List<BatchedMessage>();

        private static Task? _sendTimeout = null;

        private static readonly object _lock = new { };

        private readonly HttpClient _httpClient;

        public DefaultHttpMessageTransport(IApplicationService<HttpClient> httpClient) =>
            _httpClient = httpClient.Value ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task<Stream> SendRequest(Uri uri, Stream message, object messageObject, CancellationToken cancellationToken = default)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (messageObject == null) throw new ArgumentNullException(nameof(messageObject));

            return await ProcessRequest(uri, message);
        }

        private Task<Stream> ProcessRequest(Uri uri, Stream message)
        {
            var resultAwaiter = new TaskCompletionSource<Stream>();
            var batchedMessage = new BatchedMessage(uri, message, resultAwaiter);

            lock (_lock)
            {
                _messageBuffer.Add(batchedMessage);

                if (_sendTimeout == null) _sendTimeout = Task.Delay(_timeout).ContinueWith(x => SendBatch());

                _lastHttpClient = _httpClient;
            }

            return resultAwaiter.Task;
        }

        private void SendBatch()
        {
            IList<BatchedMessage>? sendBuffer;
            HttpClient? httpClient;

            lock (_lock)
            {
                sendBuffer = _messageBuffer;
                httpClient = _lastHttpClient!;
                _messageBuffer = new List<BatchedMessage>();
                _sendTimeout = null;
            }

            var groupedSendBatches = sendBuffer.GroupBy(x => x.Uri);

            foreach(var groupedSendBatch in groupedSendBatches)
            {
                if (groupedSendBatch.Count() == 1)
                    SendSingle(httpClient, groupedSendBatch.Single());
                else
                    SendMultiple(httpClient, groupedSendBatch);
            }
        }

        private static void SendMultiple(HttpClient httpClient, IGrouping<Uri, BatchedMessage> groupedSendBatch)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var uri = groupedSendBatch.Key;
                    var batchedMessages = groupedSendBatch.ToArray();

                    var messagePayload = batchedMessages.First().Message;
                    messagePayload.Seek(0, SeekOrigin.End);

                    var requestMessageLengths = new List<string>()
                        {
                            messagePayload.Length.ToString()
                        };
                    foreach (var additionalMessage in batchedMessages.Skip(1).ToArray())
                    {
                        requestMessageLengths.Add(additionalMessage.Message.Length.ToString());

                        await additionalMessage.Message.CopyToAsync(messagePayload);
                    }

                    messagePayload.Seek(0, SeekOrigin.Begin);
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
                    {
                        Content = new StreamContent(messagePayload)
                    };

                    requestMessage.Headers.Add(_messageLengthsHeader, requestMessageLengths);

                    var responseMessage = await httpClient.SendAsync(requestMessage);

                    responseMessage.EnsureSuccessStatusCode();

                    if (!responseMessage.Headers.TryGetValues(_messageLengthsHeader, out var responseMessageLengths))
                        throw new HttpRequestException($"Missing response header \"{_messageLengthsHeader}\"");

                    responseMessageLengths = responseMessageLengths.SelectMany(x => x.Split(','));

                    if (responseMessageLengths.Count() != batchedMessages.Length)
                        throw new HttpRequestException($"\"{_messageLengthsHeader}\" response header was present, but contained {responseMessageLengths.Count()} values when it was expected to contain {batchedMessages.Length} values");

                    var responseMessageLengthInts = new List<long>();
                    foreach (var responseMessageLength in responseMessageLengths)
                    {
                        if (!Int64.TryParse(responseMessageLength.ToString().Trim(), out var responseMessageLengthInt))
                            throw new HttpRequestException($"\"{_messageLengthsHeader}\" response header was present, but contained a non-integer value");
                        responseMessageLengthInts.Add(responseMessageLengthInt);
                    }

                    var responseContent = await responseMessage.Content.ReadAsStreamAsync();

                    var declaredLength = responseMessageLengthInts.Aggregate((x, y) => x + y);
                    if (declaredLength != responseContent.Length)
                        throw new HttpRequestException($"\"{_messageLengthsHeader}\" response header was present, but the lengths added up to {declaredLength} while the total content length is {responseContent.Length}");

                    for (var i = 0; i < batchedMessages.Length; i++)
                    {
                        var messageBytes = new byte[responseMessageLengthInts[i]];
                        await responseContent.ReadAsync(messageBytes);
                        batchedMessages[i].CompletionSource.SetResult(new MemoryStream(messageBytes));
                    }
                }
                catch (Exception exception)
                {
                    foreach (var sendBatch in groupedSendBatch)
                        sendBatch.CompletionSource.SetException(exception);
                }
            });
        }

        private void SendSingle(HttpClient httpClient, BatchedMessage batchedMessage)
        {
            _ = Task.Run(async () =>
            {
                var uri = batchedMessage.Uri;
                var messagePayload = batchedMessage.Message;

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StreamContent(messagePayload)
                };

                var responseMessage = await httpClient.SendAsync(requestMessage);

                responseMessage.EnsureSuccessStatusCode();

                var responseContent = await responseMessage.Content.ReadAsStreamAsync();

                batchedMessage.CompletionSource.SetResult(responseContent);
            });
        }

        private struct BatchedMessage
        {
            internal Uri Uri { get; }
            internal Stream Message { get; }
            internal TaskCompletionSource<Stream> CompletionSource { get; }

            internal BatchedMessage(Uri uri, Stream message, TaskCompletionSource<Stream> completionSource)
            {
                Uri = uri;
                Message = message;
                CompletionSource = completionSource;
            }
        }
    }
}
