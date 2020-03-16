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

        public Remotiatr(
            IServiceProvider internalServiceProvider,
            IServiceProvider applicationServiceProvider
        )
        {
            _internalServiceProvider = internalServiceProvider ?? throw new ArgumentNullException(nameof(internalServiceProvider));
            _applicationServiceProvider = applicationServiceProvider ?? throw new ArgumentNullException(nameof(applicationServiceProvider));
        }

        public async Task<Stream> Handle(Stream message, Uri messagePath, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (messagePath == null) throw new ArgumentNullException(nameof(messagePath));

            using var scope = _internalServiceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            var messageInfoIndex = scope.ServiceProvider.GetRequiredService<MessageInfoIndex>();
            var messageSerializer = scope.ServiceProvider.GetRequiredService<IMessageSerializer>();
            var messageHandlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IMessagePipelineHandler>>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (!messageInfoIndex.TryGetValue(messagePath, out var messageInfo))
                throw new InvalidOperationException($"No handler found for {messagePath.ToString()}");

            var data = await messageSerializer.Deserialize(message, messageInfo.RequestType);

            if (messageInfo.Type == MessageTypes.Notification)
            {
                var handler = BuildNotificationHandler(mediator, messageHandlers, cancellationToken);

                await handler(data);

                return new MemoryStream();
            }
            else
            {
                var handler = BuildRequestHandler(mediator, messageHandlers, cancellationToken);

                var response = await handler(data);

                return await messageSerializer.Serialize(response, messageInfo.ResponseType!);
            }
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
