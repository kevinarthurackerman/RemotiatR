using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RemotiatR.Shared.Internal
{
    public static class IServiceCollectionExtensions
    {
        public static void Validate(this IServiceCollection serviceCollection) => Validate(serviceCollection, new Assembly[0]);

        public static void Validate(this IServiceCollection serviceCollection, params Type[] assemblyTypeMarkers) =>
            Validate(serviceCollection, assemblyTypeMarkers.Select(x => x.Assembly).ToArray());

        public static void Validate(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            var exceptions = new List<Exception>();

            var provider = serviceCollection.BuildServiceProvider();

            var assemblyLookup = assemblies.Distinct().ToHashSet();

            var servicesToCheck = assemblies.Any()
                ? serviceCollection.Where(x => assemblyLookup.Contains(x.ServiceType.Assembly))
                : serviceCollection;

            foreach (var serviceDescriptor in servicesToCheck)
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
