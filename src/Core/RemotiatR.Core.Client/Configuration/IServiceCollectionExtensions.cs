using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace RemotiatR.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure)
            => AddRemotiatr<IDefaultRemotiatrMarker,IRemotiatr>(serviceCollection, configure);

        public static IServiceCollection AddRemotiatr<TMarker>(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure)
            => AddRemotiatr<TMarker,IRemotiatr<TMarker>>(serviceCollection, configure);

        private static IServiceCollection AddRemotiatr<TMarker,TRemotiatr>(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure)
            where TRemotiatr : IRemotiatr<TMarker>
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new AddRemotiatrOptions();
            configure.Invoke(options);

            if (options.EndpointUri == null) throw new InvalidOperationException($"{nameof(options.EndpointUri)} is a required configuration and must be set using {nameof(IAddRemotiatrOptions.SetEndpointUri)}.");

            AddDefaultServices(options);

            var notificationTypes = RegisterNotificationHandlers(
                options.AssembliesToScan,
                options.EndpointUri,
                options.Services
            ).ToArray();

            var notificationTypesLookup = ImmutableHashSet.Create(notificationTypes);

            var requestTypes = RegisterRequestHandlers(
                options.AssembliesToScan,
                options.EndpointUri,
                options.Services
            ).ToArray();

            var requestTypesLookup = ImmutableHashSet.Create(requestTypes);

            var internalServiceProvider = options.Services.BuildServiceProvider();

            serviceCollection.RemoveAll<IRemotiatr<TMarker>>();

            if (typeof(TMarker) == typeof(IDefaultRemotiatrMarker))
            {
                serviceCollection.RemoveAll<IRemotiatr>();

                serviceCollection.AddScoped<IRemotiatr>(applicationServices => new DefaultRemotiatr(internalServiceProvider, applicationServices, notificationTypesLookup, requestTypesLookup));
                serviceCollection.AddScoped(x => (IRemotiatr<TMarker>)x.GetRequiredService<IRemotiatr>());
            }
            else
            {
                serviceCollection.AddScoped<IRemotiatr<TMarker>>(applicationServices => new Remotiatr<TMarker>(internalServiceProvider, applicationServices, notificationTypesLookup, requestTypesLookup));
            }

            return serviceCollection;
        }

        private static void AddDefaultServices(AddRemotiatrOptions options)
        {
            options.Services.TryAddScoped<IApplicationServiceProviderAccessor, ApplicationServiceProviderAccessor>();

            options.Services.TryAddTransient(typeof(IApplicationService<>), typeof(ApplicationService<>));

            options.Services.AddScoped<MessageMetadata>();

            options.Services.AddMediatR(
                options.AssembliesToScan.ToArray(), 
                x =>
                {
                    switch (options.MediatorServiceLifetime)
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
                            throw new ArgumentOutOfRangeException(nameof(options.MediatorServiceLifetime), "Not a valid ServiceLifetime");
                    }

                    typeof(MediatRServiceConfiguration)
                        .GetMethod(nameof(MediatRServiceConfiguration.Using))
                        !.MakeGenericMethod(options.MediatorImplementationType)
                        .Invoke(x, new object[0]);
                }
            );
        }

        private static IEnumerable<Type> RegisterNotificationHandlers(
            IEnumerable<Assembly> assembliesToScan,
            Uri endpointUri,
            IServiceCollection serviceCollection
        )
        {
            var notificationTypes = assembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsNotificationType())
                .ToArray();

            foreach (var notificationType in notificationTypes)
            {
                var notificationHandlerInterfaceType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
                if (!serviceCollection.Any(x => x.ServiceType == notificationHandlerInterfaceType))
                {
                    var notificationHandlerType = typeof(MessageNotificationHandler<>)
                        .MakeGenericType(notificationType)
                        .GetConstructors()
                        .Single();

                    serviceCollection.TryAddTransient(
                        notificationHandlerInterfaceType,
                        x => notificationHandlerType.Invoke(new object[] 
                        { 
                            x.GetRequiredService<IMessageTransport>(),
                            x.GetRequiredService<IMessageSerializer>(),
                            x.GetRequiredService<MessageMetadata>(),
                            endpointUri 
                        })
                    );
                }
            }

            return notificationTypes;
        }

        private static IEnumerable<Type> RegisterRequestHandlers(
            IEnumerable<Assembly> assembliesToScan,
            Uri endpointUri,
            IServiceCollection serviceCollection
        )
        {
            var requestTypes = assembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsRequestType())
                .ToArray();

            foreach (var requestType in requestTypes)
            {
                var responseType = requestType.GetResponseType();

                var requestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
                if (!serviceCollection.Any(x => x.ServiceType == requestHandlerInterfaceType))
                {
                    var requestHandlerType = typeof(MessageRequestHandler<,>)
                        .MakeGenericType(requestType, responseType)
                        .GetConstructors()
                        .Single();

                    serviceCollection.TryAddTransient(
                        requestHandlerInterfaceType,
                        x => requestHandlerType.Invoke(new object[] 
                        { 
                            x.GetRequiredService<IMessageTransport>(),
                            x.GetRequiredService<IMessageSerializer>(),
                            x.GetRequiredService<MessageMetadata>(),
                            endpointUri 
                        })
                    );
                }
            }

            return requestTypes;
        }
    }
}
