using MediatR;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    internal class MessageRequestHandler<TRequest, TResponse> : BaseMessageHandler, IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public MessageRequestHandler(IMessageTransport messageTransport, IEnumerable<IMessagePipelineHandler> messageHandlers, Uri uri)
            : base(messageTransport, messageHandlers, uri) { }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var handler = BuildHandler(cancellationToken);

            var result = await handler(request);

            return (TResponse)result;
        }
    }
}
