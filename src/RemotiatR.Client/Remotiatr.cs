using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace RemotiatR.Client
{
    public class Remotiatr<TMarker> : IRemotiatr<TMarker>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IImmutableSet<Type> _notificationTypesLookup;
        private readonly IImmutableSet<Type> _requestTypesLookup;

        internal Remotiatr(IServiceProvider serviceProvider, IImmutableSet<Type> canHandleNotificationTypes, IImmutableSet<Type> canHandleRequestTypes)
        {
            _serviceProvider = serviceProvider;

            _notificationTypesLookup = canHandleNotificationTypes;
            _requestTypesLookup = canHandleRequestTypes;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default) =>
            PublishNotification(notification, cancellationToken);

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification =>
            PublishNotification(notification, cancellationToken);

        private async Task PublishNotification(object notification, CancellationToken cancellationToken)
        {
            if (!_notificationTypesLookup.TryGetValue(notification.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle notification type {notification.GetType().FullName}");

            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
            SendRequest(request, cancellationToken).ContinueWith(x => (TResponse)x.Result);

        public Task<object> Send(object request, CancellationToken cancellationToken = default) =>
            SendRequest(request, cancellationToken);

        private async Task<object> SendRequest(object request, CancellationToken cancellationToken)
        {
            if(!_requestTypesLookup.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle request type {request.GetType().FullName}");

            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            return await mediator.Send(request, cancellationToken);
        }
    }
}
