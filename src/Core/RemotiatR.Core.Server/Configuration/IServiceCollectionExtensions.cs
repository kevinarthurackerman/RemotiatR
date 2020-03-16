using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;
using System.Linq;

namespace RemotiatR.Server
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

            if (options.RootUri == null) throw new InvalidOperationException($"{nameof(options.RootUri)} is a required configuration and must be set using {nameof(IAddRemotiatrOptions.SetRootUri)}.");

            AddDefaultServices(options);

            var notificationTypes = options.AssembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsNotificationType())
                .ToArray();

            var requestTypes = options.AssembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsRequestType())
                .ToArray();

            var methodInfoLookup = notificationTypes.Concat(requestTypes)
                .Select(x => new MessageInfo(new Uri(options.RootUri, options.MessageUriLocator(x)), x))
                .ToDictionary(x => x.Path);

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
                        !.Invoke(x, new object[0]);
                }
            );
        }
    }
}
