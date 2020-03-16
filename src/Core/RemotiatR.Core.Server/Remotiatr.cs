using RemotiatR.Shared;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

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

        public async Task<HandleResult> Handle(Stream message, Uri messagePath, Attributes messageAttributes, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (messagePath == null) throw new ArgumentNullException(nameof(messagePath));

            using var scope = _internalServiceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            var messageInfoIndex = scope.ServiceProvider.GetRequiredService<MessageInfoIndex>();
            var messageSerializer = scope.ServiceProvider.GetRequiredService<IMessageSerializer>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var messageAttributesService = scope.ServiceProvider.GetRequiredService<MessageAttributes>();

            if (!messageInfoIndex.TryGetValue(messagePath, out var messageInfo))
                throw new InvalidOperationException($"No handler found for {messagePath.ToString()}");

            var data = await messageSerializer.Deserialize(message, messageInfo.RequestType);

            foreach (var attribute in messageAttributes)
                messageAttributesService.RequestAttributes.Add(attribute.Name, attribute.Value);

            if (messageInfo.Type == MessageTypes.Notification)
            {
                await mediator.Publish(data);

                return new HandleResult(new MemoryStream(), messageAttributesService.ResponseAttributes);
            }
            else
            {
                var response = await mediator.Send(data);

                var responseMessage = await messageSerializer.Serialize(response, messageInfo.ResponseType!);

                return new HandleResult(responseMessage, messageAttributesService.ResponseAttributes);
            }
        }
    }

    public class HandleResult
    {
        public Stream Message { get; }
        public Attributes MessageAttributes { get; }

        internal HandleResult(Stream message, Attributes messageAttributes)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            MessageAttributes = messageAttributes ?? throw new ArgumentNullException(nameof(messageAttributes));
        }
    }
}
