using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using RemotiatR.Shared;
using RemotiatR.Shared.Internal;
using System;
using System.Linq;

namespace RemotiatR.Server.Configuration
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions>? configure = null)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            var options = new AddRemotiatrOptions();
            configure?.Invoke(options);

            serviceCollection.TryAddSingleton<JsonSerializer, JsonSerializer>();

            serviceCollection.TryAddSingleton<ISerializer,DefaultJsonSerializer>();

            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            serviceCollection.AddMediatR(
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

            return serviceCollection;
        }
    }
}
