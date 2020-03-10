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
        private readonly IEnumerable<IHttpMessageHandler> _httpMessageHandlers;

        public DefaultHttpMessageTransport(HttpClient httpClient, IMessageSerializer serializer, IEnumerable<IHttpMessageHandler> httpMessageHandlers)
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

            var handle = BuildHandler(_httpMessageHandlers, requestMessage, cancellationToken);

            var responseMessage = await handle();

            responseMessage.EnsureSuccessStatusCode();

            var resultStream = await responseMessage.Content.ReadAsStreamAsync();

            return await _serializer.Deserialize(resultStream);
        }

        private HttpRequestHandlerDelegate BuildHandler(IEnumerable<IHttpMessageHandler> messageHandlers, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var terminalHandler = (HttpRequestHandlerDelegate)(async () => await _httpClient.SendAsync(requestMessage, cancellationToken));

            var handle = messageHandlers
                .Reverse()
                .Aggregate(terminalHandler, (next, outerHandle) => () => outerHandle.Handle(requestMessage, next, cancellationToken));

            return handle;
        }
    }
}
