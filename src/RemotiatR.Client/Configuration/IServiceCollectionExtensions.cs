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

            AddDefaultServices(options);

            var notificationTypes = RegisterNotificationHandlers(
                options.AssembliesToScan,
                options.BaseUri,
                options.UriBuilder,
                options.Services
            );

            var notificationTypesLookup = ImmutableHashSet.Create(notificationTypes.ToArray());

            var requestTypes = RegisterRequestHandlers(
                options.AssembliesToScan,
                options.BaseUri,
                options.UriBuilder,
                options.Services
            );

            var requestTypesLookup = ImmutableHashSet.Create(requestTypes.ToArray());

            var internalServiceProvider = options.Services.BuildServiceProvider();

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

        private static void AddDefaultServices(AddRemotiatrOptions addRemotiatrOptions)
        {
            addRemotiatrOptions.Services.TryAddSingleton<JsonSerializer>();

            addRemotiatrOptions.Services.TryAddSingleton<ISerializer, DefaultJsonSerializer>();

            addRemotiatrOptions.Services.TryAddSingleton<IMessageSender, DefaultHttpMessageSender>();

            addRemotiatrOptions.Services.AddMediatR(
                addRemotiatrOptions.AssembliesToScan.ToArray(), 
                x =>
                {
                    switch (addRemotiatrOptions.MediatorServiceLifetime)
                    {
                        case ServiceLifetime.Transient:
                            x.AsTransient();
                            break;
                        case ServiceLifetime.Scoped:
                            x.AsScoped();
                            break;
                        case ServiceLifetime.Singleton:
                            x.AsSingleton();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(addRemotiatrOptions.MediatorServiceLifetime), "Not a valid ServiceLifetime");
                    }
                    typeof(MediatRServiceConfiguration)
                        .GetMethod(nameof(MediatRServiceConfiguration.MediatorImplementationType))
                        .MakeGenericMethod(addRemotiatrOptions.MediatorImplementationType)
                        .Invoke(x, new object[0]);
                }
            );

            addRemotiatrOptions.Services.TryAddSingleton(x =>
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                return httpClient;
            });
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
