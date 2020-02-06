using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Immutable;

namespace RemotiatR.Client
{
    public interface IRemotiatr : IMediator
    {
    }

    public class Remotiatr : IRemotiatr
    {
        private readonly IMediator _mediator;
        private readonly ImmutableHashSet<Type> _requestTypesLookup;

        internal Remotiatr(IMediator mediator, ImmutableHashSet<Type> requestTypesLookup)
        {
            _mediator = mediator;
            _requestTypesLookup = requestTypesLookup;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default) =>
            PublishNotification(notification, cancellationToken);

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification =>
            PublishNotification(notification, cancellationToken);

        private Task PublishNotification(object request, CancellationToken cancellationToken)
        {
            if (!_requestTypesLookup.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle type {request.GetType().FullName}");

            return _mediator.Publish(request, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
            SendRequest(request, cancellationToken).ContinueWith(x => (TResponse)x.Result);

        public Task<object> Send(object request, CancellationToken cancellationToken = default) =>
            SendRequest(request, cancellationToken);

        private Task<object> SendRequest(object request, CancellationToken cancellationToken)
        {
            if(!_requestTypesLookup.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle type {request.GetType().FullName}");

            return _mediator.Send(request, cancellationToken);
        }

        private static Type GetResponseType(Type requestType) =>
            requestType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()
                .First();
    }
}
