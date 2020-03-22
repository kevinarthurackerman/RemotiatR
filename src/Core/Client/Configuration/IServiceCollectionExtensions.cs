using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;

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

            AddDefaultServices(options);

            configure.Invoke(options);

            var internalServiceProvider = options.Services.BuildServiceProvider();

            serviceCollection.RemoveAll<IRemotiatr<TMarker>>();

            if (typeof(TMarker) == typeof(IDefaultRemotiatrMarker))
            {
                serviceCollection.RemoveAll<IRemotiatr>();

                serviceCollection.AddScoped<IRemotiatr>(applicationServices =>
                    new DefaultRemotiatr(internalServiceProvider, applicationServices));

                serviceCollection.AddScoped(x => (IRemotiatr<TMarker>)x.GetRequiredService<IRemotiatr>());
            }
            else
            {
                serviceCollection.AddScoped<IRemotiatr<TMarker>>(applicationServices => 
                    new Remotiatr<TMarker>(internalServiceProvider, applicationServices));
            }

            return serviceCollection;
        }

        private static void AddDefaultServices(AddRemotiatrOptions options)
        {
            options.Services.TryAddScoped<IApplicationServiceProviderAccessor, ApplicationServiceProviderAccessor>();

            options.Services.TryAddTransient(typeof(IApplicationService<>), typeof(ApplicationService<>));

            options.Services.AddScoped<IMessageMetadata, MessageMetadata>();

            options.Services.AddSingleton<IMessageHostInfoLookup, MessageHostInfoLookup>();

            ServiceRegistrar.AddRequiredServices(options.Services, new MediatRServiceConfiguration());
        }
    }
}
