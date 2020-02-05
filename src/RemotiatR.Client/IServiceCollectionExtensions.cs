using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RemotiatR.Shared;
using System;
using System.Linq;
using System.Net.Http;

namespace RemotiatR.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection, Action<IAddRemotiatrOptions> configure = null)
        {
            var options = new AddRemotiatrOptions();
            configure?.Invoke(options);

            if (!serviceCollection.Any(x => x.ServiceType == typeof(HttpClient)))
                serviceCollection.AddSingleton<HttpClient>();

            if (!serviceCollection.Any(x => x.ServiceType == typeof(JsonSerializer)))
                serviceCollection.AddSingleton<JsonSerializer, JsonSerializer>();

            if (!serviceCollection.Any(x => x.ServiceType == typeof(ISerializer)))
                serviceCollection.AddSingleton<ISerializer,DefaultJsonSerializer>();

            if (!serviceCollection.Any(x => x.ServiceType == typeof(IMessageSender)))
                serviceCollection.AddSingleton<IMessageSender,DefaultHttpMessageSender>();

            foreach (var serverConfiguration in options.ServerConfigurations)
                serviceCollection.Add(new ServiceDescriptor(
                    serverConfiguration.Key, 
                    sp =>
                    {
                        var config = serverConfiguration.Value;
                        var messageSender = sp.GetRequiredService<IMessageSender>();
                        return new Remotiatr(messageSender, config.AssembliesToScan, config.UriBuilder);
                    }, 
                    ServiceLifetime.Singleton
                ));

            return serviceCollection;
        }
    }
}
