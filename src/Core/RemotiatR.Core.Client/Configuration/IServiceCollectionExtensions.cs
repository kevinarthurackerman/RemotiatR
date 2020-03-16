using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
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

            if (options.RootUri == null) throw new InvalidOperationException($"{nameof(options.RootUri)} is a required configuration and must be set using {nameof(IAddRemotiatrOptions.SetRootUri)}.");

            var notificationTypes = RegisterNotificationHandlers(options.AssembliesToScan, options.Services);

            var requestTypes = RegisterRequestHandlers(options.AssembliesToScan, options.Services);

            AddDefaultServices(options);

            var methodInfoLookup = notificationTypes.Concat(requestTypes)
                .Select(x => new MessageInfo(new Uri(options.RootUri, options.MessageUriLocator(x)), x))
                .ToDictionary(x => x.RequestType);

            options.Services.TryAddSingleton(new MessageInfoIndex(methodInfoLookup));

            var internalServiceProvider = options.Services.BuildServiceProvider();

            serviceCollection.RemoveAll<IRemotiatr<TMarker>>();

            if (typeof(TMarker) == typeof(IDefaultRemotiatrMarker))
            {
                serviceCollection.RemoveAll<IRemotiatr>();

                serviceCollection.AddScoped<IRemotiatr>(applicationServices => new DefaultRemotiatr(internalServiceProvider, applicationServices));
                serviceCollection.AddScoped(x => (IRemotiatr<TMarker>)x.GetRequiredService<IRemotiatr>());
            }
            else
            {
                serviceCollection.AddScoped<IRemotiatr<TMarker>>(applicationServices => new Remotiatr<TMarker>(internalServiceProvider, applicationServices));
            }

            return serviceCollection;
        }

        private static void AddDefaultServices(AddRemotiatrOptions options)
        {
            options.Services.TryAddScoped<IApplicationServiceProviderAccessor, ApplicationServiceProviderAccessor>();

            options.Services.TryAddTransient(typeof(IApplicationService<>), typeof(ApplicationService<>));

            options.Services.TryAddScoped(x => new MessageAttributes());

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
                    var notificationHandlerCtor = typeof(MessageNotificationHandler<>)
                        .MakeGenericType(notificationType)
                        .GetConstructors()
                        .Single();

                    var services = notificationHandlerCtor.GetParameters()
                        .Select(x => x.ParameterType)
                        .Select(x => (Func<IServiceProvider, object>)(y => y.GetRequiredService(x)))
                        .ToArray();

                    serviceCollection.TryAddTransient(
                        notificationHandlerInterfaceType,
                        x => notificationHandlerCtor.Invoke(services.Select(y => y.Invoke(x)).ToArray())
                    );
                }
            }

            return notificationTypes;
        }

        private static IEnumerable<Type> RegisterRequestHandlers(
            IEnumerable<Assembly> assembliesToScan,
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
                    var requestHandlerCtor = typeof(MessageRequestHandler<,>)
                        .MakeGenericType(requestType, responseType)
                        .GetConstructors()
                        .Single();

                    var services = requestHandlerCtor.GetParameters()
                        .Select(x => x.ParameterType)
                        .Select(x => (Func<IServiceProvider,object>)(y => y.GetRequiredService(x)))
                        .ToArray();

                    serviceCollection.TryAddTransient(
                        requestHandlerInterfaceType,
                        x => requestHandlerCtor.Invoke(services.Select(y => y.Invoke(x)).ToArray())
                    );
                }
            }

            return requestTypes;
        }
    }
}
