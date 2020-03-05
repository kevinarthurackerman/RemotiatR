using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.MessageTransports
{
    internal class MessageRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMessageTransport _messageTransport;
        private readonly Uri _uri;

        public MessageRequestHandler(IMessageTransport messageTransport, Uri uri)
        {
            _messageTransport = messageTransport;
            _uri = uri;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var result = await _messageTransport.SendRequest(_uri, request, typeof(TRequest), typeof(TResponse), cancellationToken);
            return (TResponse)result;
        }
    }
}
