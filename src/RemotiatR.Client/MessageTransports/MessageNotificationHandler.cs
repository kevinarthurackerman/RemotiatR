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
            _messageTransport = messageTransport;
            _uri = uri;
        }

        public Task Handle(TNotification notification, CancellationToken cancellationToken) =>
            _messageTransport.SendRequest(_uri, notification, typeof(TNotification), typeof(Unit), cancellationToken);
    }
}
