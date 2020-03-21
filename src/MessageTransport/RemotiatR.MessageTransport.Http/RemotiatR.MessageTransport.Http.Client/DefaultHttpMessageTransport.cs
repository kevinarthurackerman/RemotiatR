using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RemotiatR.Shared;
using System.IO;
using RemotiatR.Client;

namespace RemotiatR.MessageTransport.Http.Client
{
    internal class DefaultHttpMessageTransport : IMessageTransport
    {
        private readonly HttpClient _httpClient;

        public DefaultHttpMessageTransport(IApplicationService<HttpClient> httpClient) =>
            _httpClient = httpClient.Value ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task<Stream> SendRequest(Uri uri, Stream message, CancellationToken cancellationToken = default)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StreamContent(message)
            };

            var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken);

            responseMessage.EnsureSuccessStatusCode();

            return await responseMessage.Content.ReadAsStreamAsync();
        }
    }
}
