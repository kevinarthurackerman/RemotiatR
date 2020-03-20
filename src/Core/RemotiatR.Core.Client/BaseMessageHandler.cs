using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RemotiatR.Client
{
    internal abstract class BaseMessageHandler
    {
        private readonly IMessageTransport _messageTransport;
        private readonly IEnumerable<IMessagePipelineHandler> _messageHandlers;
        private readonly Uri _uri;

        public BaseMessageHandler(IMessageTransport messageTransport, IEnumerable<IMessagePipelineHandler> messageHandlers, Uri uri)
        {
            _messageTransport = messageTransport ?? throw new ArgumentNullException(nameof(messageTransport));
            _messageHandlers = messageHandlers ?? throw new ArgumentNullException(nameof(messageHandlers));
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        protected MessagePipelineDelegate BuildHandler(CancellationToken cancellationToken)
        {
            var terminalHandler = (MessagePipelineDelegate)(async message => await _messageTransport.SendRequest(_uri, message, cancellationToken));

            var handle = _messageHandlers
                .Reverse()
                .Aggregate(terminalHandler, (next, outerHandle) => async message => await outerHandle.Handle(message, next, cancellationToken));

            return handle;
        }
    }
}
