﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RemotiatR.Shared;
using RemotiatR.Client;

namespace RemotiatR.MessageTransport.Http.Client
{
    internal class DefaultHttpMessageTransport : IMessageTransport
    {
        private readonly HttpClient _httpClient;
        private readonly IMessageSerializer _serializer;

        public DefaultHttpMessageTransport(IApplicationService<HttpClient> httpClient, IMessageSerializer serializer)
        {
            _httpClient = httpClient.Value ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<object> SendRequest(Uri uri, object requestData, CancellationToken cancellationToken = default)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (requestData == null) throw new ArgumentNullException(nameof(requestData));
            
            var payload = await _serializer.Serialize(requestData);

            var content = new StreamContent(payload);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Content = content;

            var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken);

            responseMessage.EnsureSuccessStatusCode();

            var resultStream = await responseMessage.Content.ReadAsStreamAsync();

            return await _serializer.Deserialize(resultStream);
        }
    }
}
