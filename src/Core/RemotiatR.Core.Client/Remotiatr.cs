using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;

namespace RemotiatR.Client
{
    public class Remotiatr<TMarker> : IRemotiatr<TMarker>
    {
        private readonly IServiceProvider _internalServiceProvider;
        private readonly IServiceProvider _applicationServiceProvider;
        private readonly IImmutableSet<Type> _canHandleNotificationTypes;
        private readonly IImmutableSet<Type> _canHandleRequestTypes;

        internal Remotiatr(IServiceProvider internalServiceProvider, IServiceProvider applicationServiceProvider, IImmutableSet<Type> canHandleNotificationTypes, IImmutableSet<Type> canHandleRequestTypes)
        {
            _internalServiceProvider = internalServiceProvider ?? throw new ArgumentNullException(nameof(internalServiceProvider));
            _applicationServiceProvider = applicationServiceProvider ?? throw new ArgumentNullException(nameof(applicationServiceProvider));
            _canHandleNotificationTypes = canHandleNotificationTypes ?? throw new ArgumentNullException(nameof(canHandleNotificationTypes));
            _canHandleRequestTypes = canHandleRequestTypes ?? throw new ArgumentNullException(nameof(canHandleRequestTypes));
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            if (!_canHandleNotificationTypes.TryGetValue(notification.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle notification type {notification.GetType().FullName}.");

            return PublishNotification(notification, default, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            if (!_canHandleNotificationTypes.TryGetValue(notification.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle notification type {notification.GetType().FullName}.");

            return PublishNotification(notification, default, cancellationToken);
        }

        public Task Publish(object notification, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            if (!_canHandleNotificationTypes.TryGetValue(notification.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle notification type {notification.GetType().FullName}.");

            return PublishNotification(notification, configure, cancellationToken);
        }

        private async Task PublishNotification(object notification, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
        {
            using var scope = _internalServiceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            configure?.Invoke(scope.ServiceProvider);

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification, cancellationToken);
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!_canHandleRequestTypes.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle request type {request.GetType().FullName}.");

            return (TResponse)(await SendRequest(request, default, cancellationToken));
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!_canHandleRequestTypes.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle request type {request.GetType().FullName}.");

            return SendRequest(request, default, cancellationToken);
        }

        public Task<object> Send(object request, Action<IServiceProvider>? configure, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!_canHandleRequestTypes.TryGetValue(request.GetType(), out var _))
                throw new InvalidOperationException($"This server is not configured to handle request type {request.GetType().FullName}.");

            return SendRequest(request, configure, cancellationToken);
        }

        private async Task<object> SendRequest(object request, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
        {
            using var scope = _internalServiceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            configure?.Invoke(scope.ServiceProvider);

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            return await mediator.Send(request, cancellationToken);
        }
    }
}
