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
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _notificationTypesLookup = canHandleNotificationTypes ?? throw new ArgumentNullException(nameof(canHandleNotificationTypes));
            _requestTypesLookup = canHandleRequestTypes ?? throw new ArgumentNullException(nameof(canHandleRequestTypes));
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            if (!_notificationTypesLookup.TryGetValue(notification.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle notification type {notification.GetType().FullName}.");

            return PublishNotification(notification, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            if (!_notificationTypesLookup.TryGetValue(notification.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle notification type {notification.GetType().FullName}.");

            return PublishNotification(notification, cancellationToken);
        }

        private async Task PublishNotification(object notification, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification, cancellationToken);
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!_requestTypesLookup.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle request type {request.GetType().FullName}.");

            return (TResponse)(await SendRequest(request, cancellationToken));
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!_requestTypesLookup.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle request type {request.GetType().FullName}.");

            return SendRequest(request, cancellationToken);
        }

        private async Task<object> SendRequest(object request, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            return await mediator.Send(request, cancellationToken);
        }
    }
}
