using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using RemotiatR.Client.MessageSenders;
using RemotiatR.Shared;
using RemotiatR.Shared.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace RemotiatR.Client.Configuration
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure = null)
            => AddRemotiatr<IDefaultRemotiatrMarker,IRemotiatr>(serviceCollection, configure);

        public static IServiceCollection AddRemotiatr<TMarker>(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure = null)
            => AddRemotiatr<TMarker,IRemotiatr<TMarker>>(serviceCollection, configure);

        private static IServiceCollection AddRemotiatr<TMarker,TRemotiatr>(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure = null)
            where TRemotiatr : IRemotiatr<TMarker>
        {
            var options = new AddRemotiatrOptions();
            configure?.Invoke(options);

            var internalServiceCollection = BuildInternalServiceCollection(options);

            var notificationTypes = RegisterNotificationHandlers(
                options.AssembliesToScan,
                options.BaseUri,
                options.UriBuilder,
                internalServiceCollection
            );

            var notificationTypesLookup = ImmutableHashSet.Create(notificationTypes.ToArray());

            var requestTypes = RegisterRequestHandlers(
                options.AssembliesToScan,
                options.BaseUri,
                options.UriBuilder,
                internalServiceCollection
            );

            var requestTypesLookup = ImmutableHashSet.Create(requestTypes.ToArray());

            var internalServiceProvider = internalServiceCollection.BuildServiceProvider();

            serviceCollection.RemoveAll<IRemotiatr<TMarker>>();

            if (typeof(TMarker) == typeof(IDefaultRemotiatrMarker))
            {
                serviceCollection.RemoveAll<IRemotiatr>();

                serviceCollection.AddScoped<IRemotiatr>(x => new DefaultRemotiatr(internalServiceProvider, notificationTypesLookup, requestTypesLookup));
                serviceCollection.AddScoped(x => (IRemotiatr<TMarker>)x.GetRequiredService<IRemotiatr>());
            }
            else
            {
                serviceCollection.AddScoped<IRemotiatr<TMarker>>(x => new Remotiatr<TMarker>(internalServiceProvider, notificationTypesLookup, requestTypesLookup));
            }

            return serviceCollection;
        }

        private static IServiceCollection BuildInternalServiceCollection(AddRemotiatrOptions options)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            foreach (var service in options.Services) serviceCollection.Add(service);

            serviceCollection.TryAddSingleton(x =>
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                return httpClient;
            });

            serviceCollection.TryAddSingleton<JsonSerializer>();

            serviceCollection.TryAddSingleton<ISerializer, DefaultJsonSerializer>();

            serviceCollection.TryAddSingleton<IMessageSender, DefaultHttpMessageSender>();

            serviceCollection.AddMediatR(options.AssembliesToScan.ToArray());

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
