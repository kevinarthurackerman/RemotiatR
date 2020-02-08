using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    internal class MessageRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMessageSender _messageSender;
        private readonly Uri _uri;

        public MessageRequestHandler(IMessageSender messageSender, Uri uri)
        {
            _messageSender = messageSender;
            _uri = uri;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var result = await _messageSender.SendRequest(_uri, request, typeof(TRequest), typeof(TResponse), cancellationToken);
            return (TResponse)result;
        }
    }
}
