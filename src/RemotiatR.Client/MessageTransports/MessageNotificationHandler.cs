using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageTransports
{
    internal class MessageNotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        private readonly IMessageTransport _messageTransport;
        private readonly Uri _uri;

        public MessageNotificationHandler(IMessageTransport messageTransport, Uri uri)
        {
            _messageTransport = messageTransport ?? throw new ArgumentNullException(nameof(messageTransport));
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public Task Handle(TNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            return _messageTransport.SendRequest(_uri, notification, cancellationToken);
        }
    }
}
