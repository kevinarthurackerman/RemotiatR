using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;

namespace RemotiatR.Client
{
    public class Remotiatr<TMarker> : IRemotiatr<TMarker>
    {
        private readonly IServiceProvider _internalServiceProvider;
        private readonly IServiceProvider _applicationServiceProvider;

        internal Remotiatr(IServiceProvider internalServiceProvider, IServiceProvider applicationServiceProvider)
        {
            _internalServiceProvider = internalServiceProvider ?? throw new ArgumentNullException(nameof(internalServiceProvider));
            _applicationServiceProvider = applicationServiceProvider ?? throw new ArgumentNullException(nameof(applicationServiceProvider));
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            return PublishNotification(notification, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            return PublishNotification(notification, cancellationToken);
        }

        private async Task PublishNotification(object notification, CancellationToken cancellationToken)
        {
            using var scope = _internalServiceProvider.CreateScope();

            var messageInfoIndex = scope.ServiceProvider.GetRequiredService<MessageInfoIndex>();
            var applicationServiceProvider = scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (!messageInfoIndex.TryGetValue(notification.GetType(), out var messageInfo))
                throw new InvalidOperationException($"This server is not configured to handle notification type {notification.GetType().FullName}.");

            if (messageInfo.Type != MessageTypes.Notification)
                throw new InvalidOperationException($"{notification.GetType().FullName} is not a notificiation type");

            applicationServiceProvider.Value = _applicationServiceProvider;

            await mediator.Publish(notification, cancellationToken);
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return (TResponse)(await SendRequest(request, cancellationToken));
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return SendRequest(request, cancellationToken);
        }

        private async Task<object> SendRequest(object request, CancellationToken cancellationToken)
        {
            using var scope = _internalServiceProvider.CreateScope();

            var messageInfoIndex = scope.ServiceProvider.GetRequiredService<MessageInfoIndex>();
            var applicationServiceProvider = scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (!messageInfoIndex.TryGetValue(request.GetType(), out var messageInfo))
                throw new InvalidOperationException($"This server is not configured to handle request type {request.GetType().FullName}.");

            if (messageInfo.Type != MessageTypes.Request)
                throw new InvalidOperationException($"{request.GetType().FullName} is not a request type");

            applicationServiceProvider.Value = _applicationServiceProvider;

            return await mediator.Send(request, cancellationToken);
        }
    }
}
