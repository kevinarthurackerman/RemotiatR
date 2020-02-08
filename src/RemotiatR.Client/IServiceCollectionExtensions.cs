using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace RemotiatR.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure = null)
            => AddRemotiatr<Default>(serviceCollection, configure);

        public static IServiceCollection AddRemotiatr<TMarker>(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure = null)
        {
            var options = new AddRemotiatrOptions2();
            configure?.Invoke(options);

            IServiceCollection internalServiceCollection = new ServiceCollection();

            if (options.InheritServices) foreach (var service in serviceCollection) internalServiceCollection.Add(service);

            internalServiceCollection.TryAddSingleton<HttpClient>();
            internalServiceCollection.TryAddSingleton<JsonSerializer>();
            internalServiceCollection.TryAddSingleton<ISerializer, DefaultJsonSerializer>();
            internalServiceCollection.TryAddSingleton<IMessageSender,DefaultHttpMessageSender>();

            var notificationTypes = RegisterNotificationHandlers(
                options.AssembliesToScan,
                options.BaseUri,
                options.UriBuilder,
                internalServiceCollection
            );

            var requestTypes = RegisterRequestHandlers(
                options.AssembliesToScan,
                options.BaseUri,
                options.UriBuilder,
                internalServiceCollection
            );

            internalServiceCollection.AddMediatR(options.AssembliesToScan.ToArray());

            var serviceProvider = internalServiceCollection.BuildServiceProvider();

            serviceCollection.AddScoped<IRemotiatr<TMarker>>(x => new Remotiatr<TMarker>(serviceProvider, notificationTypes, requestTypes));

            return serviceCollection;
        }

        private static IEnumerable<Type> RegisterNotificationHandlers(
            IEnumerable<Assembly> assembliesToScan,
            Uri baseUri,
            Func<Type, Uri> uriBuilder,
            IServiceCollection serviceCollection
        )
        {
            var endpointInfos = assembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsNotificationType())
                .Select(x => (
                    RequestType: x,
                    Uri: MakeUriAbsolute(baseUri, uriBuilder(x))
                ))
                .ToArray();

            foreach (var endpointInfo in endpointInfos)
            {
                var notificationHandlerInterfaceType = typeof(INotificationHandler<>).MakeGenericType(endpointInfo.RequestType);
                if (!serviceCollection.Any(x => x.ServiceType == notificationHandlerInterfaceType))
                {
                    var notificationHandlerType = typeof(MessageNotificationHandler<>)
                        .MakeGenericType(endpointInfo.RequestType)
                        .GetConstructors()
                        .Single();

                    serviceCollection.AddTransient(
                        notificationHandlerInterfaceType,
                        x => notificationHandlerType.Invoke(new object[] { x.GetRequiredService<IMessageSender>(), endpointInfo.Uri })
                    );
                }
            }

            return endpointInfos.Select(x => x.RequestType);
        }

        private static IEnumerable<Type> RegisterRequestHandlers(
            IEnumerable<Assembly> assembliesToScan,
            Uri baseUri,
            Func<Type, Uri> uriBuilder,
            IServiceCollection serviceCollection
        )
        {
            var endpointInfos = assembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsRequestType())
                .Select(x => (
                    RequestType: x,
                    ResponseType: x.GetResponseType(),
                    Uri: MakeUriAbsolute(baseUri, uriBuilder(x))
                ))
                .ToArray();

            foreach (var endpointInfo in endpointInfos)
            {
                var requestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(endpointInfo.RequestType, endpointInfo.ResponseType);
                if (!serviceCollection.Any(x => x.ServiceType == requestHandlerInterfaceType))
                {
                    var requestHandlerType = typeof(MessageRequestHandler<,>)
                        .MakeGenericType(endpointInfo.RequestType, endpointInfo.ResponseType)
                        .GetConstructors()
                        .Single();

                    serviceCollection.AddTransient(
                        requestHandlerInterfaceType,
                        x => requestHandlerType.Invoke(new object[] { x.GetRequiredService<IMessageSender>(), endpointInfo.Uri })
                    );
                }
            }

            return endpointInfos.Select(x => x.RequestType);
        }

        private static Uri MakeUriAbsolute(Uri baseUri, Uri pathUri)
        {
            if (pathUri.IsAbsoluteUri) return pathUri;
            if (baseUri != null) return new Uri(baseUri, pathUri);

            throw new InvalidOperationException("If a base URI is not provided all URIs must be absolute");
        }
    }
}
