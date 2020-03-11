using MediatR;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    internal class MessageRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMessageTransport _messageTransport;
        private readonly IEnumerable<IMessagePipelineHandler> _messageHandlers;
        private readonly Uri _uri;

        public MessageRequestHandler(IMessageTransport messageTransport, IEnumerable<IMessagePipelineHandler> messageHandlers, Uri uri)
        {
            _messageTransport = messageTransport ?? throw new ArgumentNullException(nameof(messageTransport));
            _messageHandlers = messageHandlers ?? throw new ArgumentNullException(nameof(messageHandlers));
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var handler = BuildHandler(_messageTransport, _messageHandlers, cancellationToken);

            var result = await handler(request);

            return (TResponse)result;
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
