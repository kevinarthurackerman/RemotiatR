using MediatR;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageTransports
{
    internal class MessageNotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        private readonly IMessageTransport _messageTransport;
        private readonly IEnumerable<IMessagePipelineHandler> _messageHandlers;
        private readonly Uri _uri;

        public MessageNotificationHandler(IMessageTransport messageTransport, IEnumerable<IMessagePipelineHandler> messageHandlers, Uri uri)
        {
            _messageTransport = messageTransport ?? throw new ArgumentNullException(nameof(messageTransport));
            _messageHandlers = messageHandlers ?? throw new ArgumentNullException(nameof(messageHandlers)); ;
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            var handler = BuildHandler(_messageTransport, _messageHandlers, cancellationToken);

            await handler(notification);
        }

        private MessagePipelineDelegate BuildHandler(
            IMessageTransport messageTransport,
            IEnumerable<IMessagePipelineHandler> messageHandlers,
            CancellationToken cancellationToken
        )
        {
            var terminalHandler = (MessagePipelineDelegate)(async message => await messageTransport.SendRequest(_uri, message, cancellationToken));

            var handle = messageHandlers
                .Reverse()
                .Aggregate(terminalHandler, (next, outerHandle) => async message => await outerHandle.Handle(message, next, cancellationToken));

            return handle;
        }
    }
}
