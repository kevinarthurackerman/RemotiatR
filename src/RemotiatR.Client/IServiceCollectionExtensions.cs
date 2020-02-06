using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RemotiatR.Shared;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;

namespace RemotiatR.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure = null)
        {
            var options = new AddRemotiatrOptions();
            configure?.Invoke(options);

            if (!serviceCollection.Any(x => x.ServiceType == typeof(HttpClient)))
                serviceCollection.AddSingleton<HttpClient>();

            if (!serviceCollection.Any(x => x.ServiceType == typeof(JsonSerializer)))
                serviceCollection.AddSingleton<JsonSerializer, JsonSerializer>();

            if (!serviceCollection.Any(x => x.ServiceType == typeof(ISerializer)))
                serviceCollection.AddSingleton<ISerializer,DefaultJsonSerializer>();

            if (!serviceCollection.Any(x => x.ServiceType == typeof(IMessageSender)))
                serviceCollection.AddSingleton<IMessageSender,DefaultHttpMessageSender>();

            foreach (var serverConfigurations in options.ServerConfigurations)
            {
                var serverType = serverConfigurations.Key;
                var configs = serverConfigurations.Value;

                var endpointInfos = configs.AssembliesToScan
                    .SelectMany(x => x.GetTypes())
                    .Where(x =>
                        x.IsClass
                        && x.IsVisible
                        && x.GetConstructors().Any(x => !x.IsStatic && x.IsPublic)
                        && x.GetInterfaces().Any(x => x == typeof(INotification) || x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
                    )
                    .Select(x => (
                        RequestType: x,
                        ResponseType: GetResponseType(x),
                        Uri: MakeUriAbsolute(configs.BaseUri, configs.UriBuilder(x))
                    ))
                    .ToArray();

                foreach (var endpointInfo in endpointInfos)
                {
                    if (endpointInfo.RequestType.GetInterfaces().Any(x => x == typeof(INotification)))
                    {
                        var notificationHandlerInterfaceType = typeof(INotificationHandler<>).MakeGenericType(endpointInfo.RequestType);
                        if (!serviceCollection.Any(x => x.ServiceType == notificationHandlerInterfaceType))
                        {
                            var notificationHandlerType = typeof(MessageNotificationHandler<>)
                                .MakeGenericType(endpointInfo.RequestType)
                                .GetConstructors().Single();
                            serviceCollection.AddTransient(
                                notificationHandlerInterfaceType,
                                x => notificationHandlerType.Invoke(new object[] { configs.MessageSenderLocator(x), endpointInfo.Uri })
                            );
                        }
                    }
                    else
                    {
                        var requestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(endpointInfo.RequestType, endpointInfo.ResponseType);
                        if (!serviceCollection.Any(x => x.ServiceType == requestHandlerInterfaceType))
                        {
                            var requestHandlerType = typeof(MessageRequestHandler<,>)
                                .MakeGenericType(endpointInfo.RequestType, endpointInfo.ResponseType)
                                .GetConstructors().Single();
                            serviceCollection.AddTransient(
                                requestHandlerInterfaceType,
                                x => requestHandlerType.Invoke(new object[] { configs.MessageSenderLocator(x), endpointInfo.Uri })
                            );
                        }
                    }
                }

                var requestTypesLookup = ImmutableHashSet.Create(endpointInfos.Select(x => x.RequestType).ToArray());

                serviceCollection.Add(new ServiceDescriptor(
                    serverType,
                    sp =>
                    {
                        var mediator = sp.GetRequiredService<IMediator>();
                        return new Remotiatr(mediator, requestTypesLookup);
                    },
                    ServiceLifetime.Singleton
                ));

                static Uri MakeUriAbsolute(Uri baseUri, Uri pathUri)
                {
                    if (pathUri.IsAbsoluteUri) return pathUri;
                    if (baseUri != null) return new Uri(baseUri, pathUri);

                    throw new InvalidOperationException("If a base URI is not provided all URIs must be absolute");
                }
            }

            return serviceCollection;

            static Type GetResponseType(Type requestType) =>
                requestType.GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
                    .GetGenericArguments()
                    .First();
        }
    }
}
