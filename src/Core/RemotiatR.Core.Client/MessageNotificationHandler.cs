using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    internal class MessageNotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        private readonly IMessageTransport _messageTransport;

        public MessageNotificationHandler(IMessageTransport messageTransport) =>
            _messageTransport = messageTransport ?? throw new ArgumentNullException(nameof(messageTransport));

        public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            await _messageTransport.SendRequest(notification);
        }
    }
}
