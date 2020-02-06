using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace RemotiatR.Shared
{
    public static class IServiceCollectionExtensions
    {
        public static bool TryAddSingleton<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceCollection.Any(x => x.ServiceType == typeof(TService))) return false;
            serviceCollection.AddSingleton<TService, TImplementation>();
            return true;
        }

        public static bool TryAddSingleton<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection.Any(x => x.ServiceType == typeof(TService))) return false;
            serviceCollection.AddSingleton<TService>();
            return true;
        }
    }
}
