using RemotiatR.Shared;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

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

        public async Task<Stream> Handle(Stream message, CancellationToken cancellationToken = default)
        {
            if(message == null) throw new ArgumentNullException(nameof(message));

            using var scope = _internalServiceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            var messageSerializer = scope.ServiceProvider.GetRequiredService<IMessageSerializer>();
            var messageHandlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IMessagePipelineHandler>>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var data = await messageSerializer.Deserialize(message);

            if(_canHandleNotificationTypes.Contains(data.GetType()))
            {
                var handler = BuildNotificationHandler(mediator, messageHandlers, cancellationToken);

                await handler(data);

                return new MemoryStream();
            }

            if (_canHandleRequestTypes.Contains(data.GetType()))
            {
                var handler = BuildRequestHandler(mediator, messageHandlers, cancellationToken);

                var response = await handler(data);

                return await messageSerializer.Serialize(response);
            }

            throw new InvalidOperationException($"Type {data.GetType()} is neither a {typeof(INotification).FullName} nor an {nameof(IRequest)}");
        }

        private MessagePipelineDelegate BuildNotificationHandler(
            IMediator mediator,
            IEnumerable<IMessagePipelineHandler> messageHandlers,
            CancellationToken cancellationToken
        )
        {
            var terminalHandler = (MessagePipelineDelegate)(async message => 
            {
                await mediator.Publish(message, cancellationToken);
                return Task.CompletedTask;
            });

            var handle = messageHandlers
                .Reverse()
                .Aggregate(terminalHandler, (next, outerHandle) => async message => await outerHandle.Handle(message, next, cancellationToken));

            return handle;
        }

        private MessagePipelineDelegate BuildRequestHandler(
            IMediator mediator,
            IEnumerable<IMessagePipelineHandler> messageHandlers,
            CancellationToken cancellationToken
        )
        {
            var terminalHandler = (MessagePipelineDelegate)(async message => await mediator.Send(message, cancellationToken));

            var handle = messageHandlers
                .Reverse()
                .Aggregate(terminalHandler, (next, outerHandle) => async message => await outerHandle.Handle(message, next, cancellationToken));

            return handle;
        }
    }
}
