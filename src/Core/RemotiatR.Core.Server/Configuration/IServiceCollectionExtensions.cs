using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using RemotiatR.Shared.Internal;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace RemotiatR.Server.Configuration
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure)
            => AddRemotiatr<IDefaultRemotiatrMarker, IRemotiatr>(serviceCollection, configure);

        public static IServiceCollection AddRemotiatr<TMarker>(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure)
            => AddRemotiatr<TMarker, IRemotiatr<TMarker>>(serviceCollection, configure);

        private static IServiceCollection AddRemotiatr<TMarker, TRemotiatr>(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure)
            where TRemotiatr : IRemotiatr<TMarker>
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new AddRemotiatrOptions();
            configure.Invoke(options);

            AddDefaultServices(options);

            var notificationTypes = options.AssembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsNotificationType())
                .ToArray();

            var notificationTypesLookup = ImmutableHashSet.Create(notificationTypes);

            var requestTypes = options.AssembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsRequestType())
                .ToArray();

            var requestTypesLookup = ImmutableHashSet.Create(requestTypes);

            var responseTypes = requestTypes
                .Select(x => x.GetResponseType())
                .ToArray();

            var keyMessageTypeMappings = notificationTypes.Concat(requestTypes).Concat(responseTypes)
                .Distinct()
                .Select(x => new KeyMessageTypeMapping(options.MessageKeyGenerator(x), x))
                .ToArray();

            foreach (var keyMessageTypeMapping in keyMessageTypeMappings)
                options.Services.AddSingleton(keyMessageTypeMapping);

            var internalServiceProvider = options.Services.BuildServiceProvider();

            serviceCollection.RemoveAll<IRemotiatr<TMarker>>();

            if (typeof(TMarker) == typeof(IDefaultRemotiatrMarker))
            {
                serviceCollection.RemoveAll<IRemotiatr>();

                serviceCollection.AddSingleton<IRemotiatr>(x => new DefaultRemotiatr(internalServiceProvider, notificationTypesLookup, requestTypesLookup));
                serviceCollection.AddSingleton(x => (IRemotiatr<TMarker>)x.GetRequiredService<IRemotiatr>());
            }
            else
            {
                serviceCollection.AddSingleton<IRemotiatr<TMarker>>(x => new Remotiatr<TMarker>(internalServiceProvider, notificationTypesLookup, requestTypesLookup));
            }

            return serviceCollection;
        }

        private static void AddDefaultServices(AddRemotiatrOptions options)
        {
            options.Services.TryAddSingleton<IMessageSerializer, DefaultJsonMessageSerializer>();

            options.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            options.Services.TryAddSingleton<IKeyMessageTypeMappings, KeyMessageTypeMappings>();

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
                        !.Invoke(x, new object[0]);
                }
            );
        }
    }
}
