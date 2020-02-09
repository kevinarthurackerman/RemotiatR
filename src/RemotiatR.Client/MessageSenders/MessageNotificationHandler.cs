using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageSenders
{
    internal class MessageNotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        private readonly IMessageSender _messageSender;
        private readonly Uri _uri;

        public MessageNotificationHandler(IMessageSender messageSender, Uri uri)
        {
            _messageSender = messageSender;
            _uri = uri;
        }

        public Task Handle(TNotification notification, CancellationToken cancellationToken) =>
            _messageSender.SendRequest(_uri, notification, typeof(TNotification), typeof(Unit), cancellationToken);
    }
}
