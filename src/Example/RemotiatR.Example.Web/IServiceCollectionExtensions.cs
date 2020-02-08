using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace RemotiatR.Example.Web
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClient(this IServiceCollection serviceCollection, Action<IList<DelegatingHandler>> configure = null)
        {
            var handlers = new List<DelegatingHandler>();
            configure?.Invoke(handlers);

            var existingClient = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(HttpClient));
            if (existingClient != null) serviceCollection.Remove(existingClient);

            serviceCollection.AddSingleton(x => HttpClientFactory.Create(handlers.ToArray()));

            return serviceCollection;
        }
    }
}
