using MediatR;
using RemotiatR.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    internal class MessageRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMessageTransport _messageTransport;
        private readonly IMessageSerializer _messageSerializer;
        private readonly MessageMetadata _messageMetadata;
        private readonly Uri _uri;

        public MessageRequestHandler(
            IMessageTransport messageTransport,
            IMessageSerializer messageSerializer,
            MessageMetadata messageMetadata,
            Uri uri
        )
        {
            _messageTransport = messageTransport ?? throw new ArgumentNullException(nameof(messageTransport));
            _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
            _messageMetadata = messageMetadata ?? throw new ArgumentNullException(nameof(messageMetadata));
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var requestMessagePayloadStream = (await _messageSerializer.Serialize(request, _messageMetadata.RequestMetadata))
                ?? throw new InvalidOperationException("Request message payload was serialized to a null value");

            var responseMessagePayloadStream = await _messageTransport.SendRequest(_uri, requestMessagePayloadStream, cancellationToken);

            var messageSerializerResult = (await _messageSerializer.Deserialize(responseMessagePayloadStream))
                ?? throw new InvalidOperationException("Response message payload was deserialized to a null value");

            foreach (var metaData in messageSerializerResult.MessageMetadata)
                _messageMetadata.ResponseMetadata.Add(metaData);

#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
            if (messageSerializerResult.Message == null) return default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.

            return (TResponse)Convert.ChangeType(messageSerializerResult.Message, typeof(TResponse));
        }
    }
}
