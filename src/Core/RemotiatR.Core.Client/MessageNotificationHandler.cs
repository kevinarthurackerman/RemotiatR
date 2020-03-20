using MediatR;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    internal class MessageNotificationHandler<TNotification> : BaseMessageHandler, INotificationHandler<TNotification>
        where TNotification : INotification
    {
        public MessageNotificationHandler(IMessageTransport messageTransport, IEnumerable<IMessagePipelineHandler> messageHandlers, Uri uri)
            : base(messageTransport, messageHandlers, uri) { }

        public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            var handler = BuildHandler(cancellationToken);

            await handler(notification);
        }
    }
}
