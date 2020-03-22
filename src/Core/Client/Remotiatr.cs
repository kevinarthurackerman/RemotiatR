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

            return PublishNotification(notification, default, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            return PublishNotification(notification, default, cancellationToken);
        }

        public Task Publish(object notification, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            return PublishNotification(notification, configure, cancellationToken);
        }

        private async Task PublishNotification(object notification, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
        {
            using var scope = _internalServiceProvider.CreateScope();

            CheckIfCanHandleMessageType(notification, scope.ServiceProvider, MediatorTypes.Notification);

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            configure?.Invoke(scope.ServiceProvider);

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification, cancellationToken);
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return (TResponse)(await SendRequest(request, default, cancellationToken));
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return SendRequest(request, default, cancellationToken);
        }

        public Task<object> Send(object request, Action<IServiceProvider>? configure, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return SendRequest(request, configure, cancellationToken);
        }

        private async Task<object> SendRequest(object request, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
        {
            using var scope = _internalServiceProvider.CreateScope();

            CheckIfCanHandleMessageType(request, scope.ServiceProvider, MediatorTypes.Request);

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            configure?.Invoke(scope.ServiceProvider);

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            return await mediator.Send(request, cancellationToken);
        }

        private static void CheckIfCanHandleMessageType(object message, IServiceProvider serviceProvider, MediatorTypes mediatorType)
        {
            var messageHostInfoLookup = serviceProvider.GetRequiredService<IMessageHostInfoLookup>();

            if (!messageHostInfoLookup.TryGetValue(message.GetType(), out var messageHostInfo))
                throw new InvalidOperationException($"No host found that is configured to handle message type {message.GetType().FullName}");

            if (!messageHostInfo.MessageInfos.TryGetValue(message.GetType(), out var messageInfo))
                throw new InvalidOperationException($"A host was found that is configured to handle message type {message.GetType().FullName}, but an unexpected error occurred");

            if (messageInfo.MediatorType != mediatorType)
                throw new InvalidOperationException($"Type {message.GetType().FullName} is configured as a {Enum.GetName(typeof(MediatorTypes), messageInfo.MediatorType)}, but was sent as a {Enum.GetName(typeof(MediatorTypes), mediatorType)}");
        }
    }
}
