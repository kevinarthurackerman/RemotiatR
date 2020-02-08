using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace RemotiatR.Client
{
    public interface IMessageSender
    {
        Task<object> SendRequest(Uri uri, object requestData, Type requestType, Type responseType, CancellationToken cancellationToken);
    }

    internal class DefaultHttpMessageSender : IMessageSender
    {
        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly IEnumerable<IHttpMessageHandler> _httpMessageHandlers;

        public DefaultHttpMessageSender(HttpClient httpClient, ISerializer serializer, IEnumerable<IHttpMessageHandler> httpMessageHandlers)
        {
            _httpClient = httpClient;
            _serializer = serializer;
            _httpMessageHandlers = httpMessageHandlers;
        }

        public async Task<object> SendRequest(Uri uri, object requestData, Type requestType, Type responseType, CancellationToken cancellationToken)
        {
            var payload = _serializer.Serialize(requestData, requestType);

            var content = new StreamContent(payload);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Content = content;

            HttpRequestHandlerDelegate terminalHandler = async () => await _httpClient.SendAsync(requestMessage, cancellationToken);

            var handle = _httpMessageHandlers
                .Reverse()
                .Aggregate(terminalHandler, (next, outerHandle) => () => outerHandle.Handle(requestMessage, cancellationToken, next));

            var responseMessage = await handle();

            if ((int)responseMessage.StatusCode >= 200 && (int)responseMessage.StatusCode < 300)
            {
                var resultStream = await responseMessage.Content.ReadAsStreamAsync();

                return _serializer.Deserialize(resultStream, responseType);
            }

            return null;
        }
    }

    public interface IHttpMessageHandler
    {
        Task<HttpResponseMessage> Handle(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken, HttpRequestHandlerDelegate next);
    }

    public delegate Task<HttpResponseMessage> HttpRequestHandlerDelegate();
}
