using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RemotiatR.Shared;
using System.Linq;

namespace RemotiatR.Server
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection)
        {
            if (!serviceCollection.Any(x => x.ServiceType == typeof(JsonSerializer)))
                serviceCollection.AddSingleton<JsonSerializer, JsonSerializer>();

            if (!serviceCollection.Any(x => x.ServiceType == typeof(ISerializer)))
                serviceCollection.AddSingleton<ISerializer,DefaultJsonSerializer>();

            return serviceCollection;
        }
    }
}
