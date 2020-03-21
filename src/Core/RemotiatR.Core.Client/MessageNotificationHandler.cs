using MediatR;
using RemotiatR.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    internal class MessageNotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        private readonly IMessageTransport _messageTransport;
        private readonly IMessageSerializer _messageSerializer;
        private readonly MessageMetadata _messageMetadata;
        private readonly Uri _uri;

        public MessageNotificationHandler(
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

        public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            var requestMessagePayloadStream = (await _messageSerializer.Serialize(notification, _messageMetadata.RequestMetadata))
                ?? throw new InvalidOperationException("Request message payload was serialized to a null value");

            var responseMessagePayloadStream = await _messageTransport.SendRequest(_uri, requestMessagePayloadStream, cancellationToken);

            var messageSerializerResult = (await _messageSerializer.Deserialize(responseMessagePayloadStream))
                ?? throw new InvalidOperationException("Response message payload was deserialized to a null value");

            foreach (var metaData in messageSerializerResult.MessageMetadata)
                _messageMetadata.ResponseMetadata.Add(metaData);

            if (messageSerializerResult.Message != null)
                throw new InvalidOperationException($"Response message contained a message payload, but was not expected to");
        }
    }
}
