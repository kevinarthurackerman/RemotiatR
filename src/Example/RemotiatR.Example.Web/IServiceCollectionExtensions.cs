using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RemotiatR.Example.Web
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClient(this IServiceCollection serviceCollection, Action<IList<DelegatingHandler>> configure = null) =>
            AddHttpClient<HttpClient>(serviceCollection, configure);

        public static IServiceCollection AddHttpClient<TClient>(this IServiceCollection serviceCollection, Action<IList<DelegatingHandler>> configure = null)
            where TClient : HttpClient
        {
            var handlers = new List<DelegatingHandler>();
            configure?.Invoke(handlers);

            serviceCollection.AddSingleton(x => HttpClientFactory.Create(handlers.ToArray()));

            return serviceCollection;
        }
    }
}
