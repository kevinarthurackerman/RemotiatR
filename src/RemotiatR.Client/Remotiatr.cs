using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace RemotiatR.Client
{
    public interface IRemotiatr : IMediator
    {
    }

    public class Remotiatr : IRemotiatr
    {
        private readonly IMediator _mediator;
        private readonly ImmutableHashSet<Type> _notificationTypesLookup;
        private readonly ImmutableHashSet<Type> _requestTypesLookup;

        internal Remotiatr(IMediator mediator, IEnumerable<Type> notificationTypes, IEnumerable<Type> requestTypes)
        {
            _mediator = mediator;
            _notificationTypesLookup = ImmutableHashSet.Create(notificationTypes.ToArray());
            _requestTypesLookup = ImmutableHashSet.Create(requestTypes.ToArray());
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default) =>
            PublishNotification(notification, cancellationToken);

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification =>
            PublishNotification(notification, cancellationToken);

        private Task PublishNotification(object notification, CancellationToken cancellationToken)
        {
            if (!_notificationTypesLookup.TryGetValue(notification.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle notification type {notification.GetType().FullName}");

            return _mediator.Publish(notification, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
            SendRequest(request, cancellationToken).ContinueWith(x => (TResponse)x.Result);

        public Task<object> Send(object request, CancellationToken cancellationToken = default) =>
            SendRequest(request, cancellationToken);

        private Task<object> SendRequest(object request, CancellationToken cancellationToken)
        {
            if(!_requestTypesLookup.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle request type {request.GetType().FullName}");

            return _mediator.Send(request, cancellationToken);
        }
    }
}
