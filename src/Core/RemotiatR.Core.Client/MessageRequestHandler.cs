using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    internal class MessageRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMessageTransport _messageTransport;

        public MessageRequestHandler(IMessageTransport messageTransport) =>
            _messageTransport = messageTransport ?? throw new ArgumentNullException(nameof(messageTransport));

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = await _messageTransport.SendRequest(request);

            return (TResponse)result;
        }
    }
}
