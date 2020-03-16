using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RemotiatR.Shared;
using RemotiatR.Client;
using MediatR;
using System.Linq;

namespace RemotiatR.MessageTransport.Http.Client
{
    internal class DefaultHttpMessageTransport : IMessageTransport
    {
        private const string _headerPrefix = "remotiatr-";

        private readonly HttpClient _httpClient;
        private readonly IMessageSerializer _serializer;
        private readonly MessageInfoIndex _messageInfoIndex;
        private readonly MessageAttributes _messageAttributes;

        public DefaultHttpMessageTransport(
            IApplicationService<HttpClient> httpClient,
            IMessageSerializer serializer,
            MessageInfoIndex messageInfoIndex,
            MessageAttributes messageAttributes
        )
        {
            _httpClient = httpClient.Value ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _messageInfoIndex = messageInfoIndex ?? throw new ArgumentNullException(nameof(messageInfoIndex));
            _messageAttributes = messageAttributes ?? throw new ArgumentNullException(nameof(messageAttributes));
        }

        public async Task<object> SendRequest(object requestData, CancellationToken cancellationToken = default)
        {
            if (requestData == null) throw new ArgumentNullException(nameof(requestData));

            if (!_messageInfoIndex.TryGetValue(requestData.GetType(), out var messageInfo))
                throw new InvalidOperationException($"No handler registered for type {requestData.GetType().FullName}");
            
            var payload = await _serializer.Serialize(requestData, messageInfo.RequestType);

            var content = new StreamContent(payload);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, messageInfo.Path);
            requestMessage.Content = content;

            foreach (var requestAttribute in _messageAttributes.RequestAttributes)
            {
                var name = _headerPrefix + requestAttribute.Key;
                requestMessage.Headers.Add(name, requestAttribute.Value ?? String.Empty);
            }

            var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken);

            responseMessage.EnsureSuccessStatusCode();

            foreach(var responseHeader in responseMessage.Headers.Where(x => x.Key.StartsWith(_headerPrefix)))
            {
                var name = responseHeader.Key.Substring(_headerPrefix.Length);
                _messageAttributes.ResponseAttributes.Add(name, responseHeader.Value.Single().ToString() ?? String.Empty);
            }

            var resultStream = await responseMessage.Content.ReadAsStreamAsync();

            if (messageInfo.Type == MessageTypes.Notification) return Unit.Value;

            return await _serializer.Deserialize(resultStream, messageInfo.ResponseType!);
        }
    }
}
