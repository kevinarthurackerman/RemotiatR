using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static void Validate(this IServiceCollection serviceCollection) => Validate(serviceCollection, Assembly.GetCallingAssembly());

        public static void Validate(this IServiceCollection serviceCollection, params Type[] assemblyTypeMarkers) =>
            Validate(serviceCollection, assemblyTypeMarkers.Select(x => x.Assembly).ToArray());

        public static void Validate(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            var exceptions = new List<Exception>();
            var provider = serviceCollection.BuildServiceProvider();
            var assemblyLookup = assemblies.Distinct().ToHashSet();
            foreach (var serviceDescriptor in serviceCollection.Where(x => assemblyLookup.Contains(x.ServiceType.Assembly)))
            {
                try
                {
                    provider.GetService(serviceDescriptor.ServiceType);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException("Missing some service dependencies.", exceptions);
            }
        }
    }
}
