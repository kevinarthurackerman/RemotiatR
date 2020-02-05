using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    public interface IMessageSender
    {
        Task<object> SendRequest(string url, object requestData, Type requestType, Type responseType, CancellationToken cancellationToken);
    }

    public class DefaultHttpMessageSender : IMessageSender
    {
        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;

        public DefaultHttpMessageSender(HttpClient httpClient, ISerializer serializer)
        {
            _httpClient = httpClient;
            _serializer = serializer;
        }

        public async Task<object> SendRequest(string url, object requestData, Type requestType, Type responseType, CancellationToken cancellationToken)
        {
            var payload = _serializer.Serialize(requestData, requestType);

            var content = new StreamContent(payload);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMessage = await _httpClient.PostAsync(url, content, cancellationToken);

            var resultStream = await responseMessage.Content.ReadAsStreamAsync();

            var result = _serializer.Deserialize(resultStream, responseType);

            return result;
        }
    }
}
