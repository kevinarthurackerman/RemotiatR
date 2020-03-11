using RemotiatR.Shared;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace RemotiatR.Server
{
    internal class Remotiatr<TMarker> : IRemotiatr<TMarker>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IImmutableSet<Type> _canHandleNotificationTypes;
        private readonly IImmutableSet<Type> _canHandleRequestTypes;

        public Remotiatr(IServiceProvider serviceProvider, IImmutableSet<Type> canHandleNotificationTypes, IImmutableSet<Type> canHandleRequestTypes)
        {
            _serviceProvider = serviceProvider;
            _canHandleNotificationTypes = canHandleNotificationTypes;
            _canHandleRequestTypes = canHandleRequestTypes;
        }

        public Task<Stream> Handle(Stream message) => Handle(message, default, default);

        public Task<Stream> Handle(Stream message, CancellationToken cancellationToken) => Handle(message, default, cancellationToken);

        public Task<Stream> Handle(Stream message, Action<IServiceProvider> configureServices) => Handle(message, configureServices, default);

        public async Task<Stream> Handle(Stream message, Action<IServiceProvider> configureServices, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            configureServices?.Invoke(scope.ServiceProvider);

            var messageSerializer = scope.ServiceProvider.GetRequiredService<IMessageSerializer>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var data = await messageSerializer.Deserialize(message);

            if(_canHandleNotificationTypes.Contains(data.GetType()))
            {
                await mediator.Publish(data, cancellationToken);
                return new MemoryStream();
            }

            if (_canHandleRequestTypes.Contains(data.GetType()))
            {
                var response = await mediator.Send(data, cancellationToken);
                return await messageSerializer.Serialize(response);
            }

            throw new InvalidOperationException($"Type {data.GetType()} is neither a {typeof(INotification).FullName} nor an {nameof(IRequest)}");
        }
    }
}
