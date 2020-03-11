using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using RemotiatR.Shared;

namespace RemotiatR.Client.MessageTransports
{
    internal class DefaultHttpMessageTransport : IMessageTransport
    {
        private readonly HttpClient _httpClient;
        private readonly IMessageSerializer _serializer;
        private readonly IEnumerable<IHttpMessagePipelineHandler> _httpMessageHandlers;

        public DefaultHttpMessageTransport(HttpClient httpClient, IMessageSerializer serializer, IEnumerable<IHttpMessagePipelineHandler> httpMessageHandlers)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _httpMessageHandlers = httpMessageHandlers ?? throw new ArgumentNullException(nameof(httpMessageHandlers));
        }

        public async Task<object> SendRequest(Uri uri, object requestData, CancellationToken cancellationToken = default)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (requestData == null) throw new ArgumentNullException(nameof(requestData));

            var payload = await _serializer.Serialize(requestData);

            var content = new StreamContent(payload);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Content = content;

            var handle = BuildHandler(_httpClient, _httpMessageHandlers, requestMessage, cancellationToken);

            var responseMessage = await handle();

            responseMessage.EnsureSuccessStatusCode();

            var resultStream = await responseMessage.Content.ReadAsStreamAsync();

            return await _serializer.Deserialize(resultStream);
        }

        private HttpRequestPipelineDelegate BuildHandler(
            HttpClient httpClient,
            IEnumerable<IHttpMessagePipelineHandler> messageHandlers,
            HttpRequestMessage requestMessage,
            CancellationToken cancellationToken
        )
        {
            var terminalHandler = (HttpRequestPipelineDelegate)(async () => await httpClient.SendAsync(requestMessage, cancellationToken));

            var handle = messageHandlers
                .Reverse()
                .Aggregate(terminalHandler, (next, outerHandle) => async () => await outerHandle.Handle(requestMessage, next, cancellationToken));

            return handle;
        }
    }
}
