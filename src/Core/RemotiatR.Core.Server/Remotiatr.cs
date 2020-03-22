using RemotiatR.Shared;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace RemotiatR.Server
{
    internal class Remotiatr<TMarker> : IRemotiatr<TMarker>
    {
        private readonly IServiceProvider _internalServiceProvider;
        private readonly IServiceProvider _applicationServiceProvider;

        public Remotiatr(IServiceProvider internalServiceProvider, IServiceProvider applicationServiceProvider)
        {
            _internalServiceProvider = internalServiceProvider ?? throw new ArgumentNullException(nameof(internalServiceProvider));
            _applicationServiceProvider = applicationServiceProvider ?? throw new ArgumentNullException(nameof(applicationServiceProvider));
        }

        public async Task<Stream> Handle(Uri uri, Stream message, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            using var scope = _internalServiceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IApplicationServiceProviderAccessor>().Value = _applicationServiceProvider;

            configure?.Invoke(scope.ServiceProvider);

            var messageMetadata = scope.ServiceProvider.GetRequiredService<IMessageMetadata>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var messageHostInfoLookup = scope.ServiceProvider.GetRequiredService<IMessageHostInfoLookup>();

            var hostUri = new Uri($"{uri.Scheme}://{uri.Authority}");

            if (!messageHostInfoLookup.TryGetValue(hostUri, out var messageHostInfo))
                throw new InvalidOperationException($"No host found that is configured to handle uri {hostUri.ToString()}");

            var messageSerializer = (IMessageSerializer)scope.ServiceProvider.GetRequiredService(messageHostInfo.MessageSerializerType);

            var messageDeserializeResult = (await messageSerializer.Deserialize(message)
                ?? throw new InvalidOperationException("Message was parsed to a null value and could not be handled"));

            var messageData = messageDeserializeResult.Message ?? throw new InvalidOperationException("Message was parsed but contained no data");

            if (!messageHostInfo.MessageInfos.TryGetValue(messageData.GetType(), out var messageInfo))
                throw new InvalidOperationException($"A host was found that is configured to handle root uri {hostUri.ToString()}, but it has no handler for message type {messageData.GetType().FullName}");

            var messageUri = new Uri(hostUri, messageInfo.EndpointPath);
            if (messageUri != uri)
                throw new InvalidOperationException($"A host was found that is configured to handle message type {messageData.GetType().FullName} at uri {messageUri.ToString()}, but received it at {uri.ToString()}");

            foreach (var metadata in messageDeserializeResult.MessageMetadata)
                messageMetadata.RequestMetadata.Add(metadata);
               
            if (messageInfo.MediatorType == MediatorTypes.Notification)
            {
                await mediator.Publish(messageData, cancellationToken);

                return await messageSerializer.Serialize(null, messageMetadata.ResponseMetadata);
            }

            if (messageInfo.MediatorType == MediatorTypes.Request)
            {
                var responseData = await mediator.Send(messageData, cancellationToken);

                return await messageSerializer.Serialize(responseData, messageMetadata.ResponseMetadata);
            }

            throw new InvalidOperationException("Host failed to handle request unexpectedly");
        }
    }
}
