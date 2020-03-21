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
        private readonly IServiceProvider _internalServiceProvider;
        private readonly IServiceProvider _applicationServiceProvider;
        private readonly IImmutableSet<Type> _canHandleNotificationTypes;
        private readonly IImmutableSet<Type> _canHandleRequestTypes;

        public Remotiatr(
            IServiceProvider internalServiceProvider,
            IServiceProvider applicationServiceProvider,
            IImmutableSet<Type> canHandleNotificationTypes, 
            IImmutableSet<Type> canHandleRequestTypes
        )
        {
            _internalServiceProvider = internalServiceProvider ?? throw new ArgumentNullException(nameof(internalServiceProvider));
            _applicationServiceProvider = applicationServiceProvider ?? throw new ArgumentNullException(nameof(applicationServiceProvider));
            _canHandleNotificationTypes = canHandleNotificationTypes ?? throw new ArgumentNullException(nameof(canHandleNotificationTypes));
            _canHandleRequestTypes = canHandleRequestTypes ?? throw new ArgumentNullException(nameof(canHandleRequestTypes));
        }

        public async Task<Stream> Handle(Stream message, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
        {
            if(message == null) throw new ArgumentNullException(nameof(message));

            using var scope = _internalServiceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            configure?.Invoke(scope.ServiceProvider);

            var messageSerializer = scope.ServiceProvider.GetRequiredService<IMessageSerializer>();
            var messageMetadata = scope.ServiceProvider.GetRequiredService<MessageMetadata>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var messagePayloadResult = (await messageSerializer.Deserialize(message)
                ?? throw new InvalidOperationException("Message was parsed to a null value and could not be handled"));

            var messageData = messagePayloadResult.Message ?? throw new InvalidOperationException("Message was parsed but contained no data");

            foreach (var metadata in messagePayloadResult.MessageMetadata)
                messageMetadata.RequestMetadata.Add(metadata);

            if (_canHandleNotificationTypes.Contains(messageData.GetType()))
            {
                await mediator.Publish(messageData, cancellationToken);

                return await messageSerializer.Serialize(null, messageMetadata.ResponseMetadata);
            }

            if (_canHandleRequestTypes.Contains(messageData.GetType()))
            {
                var responseData = await mediator.Send(messageData, cancellationToken);

                return await messageSerializer.Serialize(responseData, messageMetadata.ResponseMetadata);
            }

            throw new InvalidOperationException($"Type {messageData.GetType()} is neither a {typeof(INotification).FullName} nor an {nameof(IRequest)}");
        }
    }
}
