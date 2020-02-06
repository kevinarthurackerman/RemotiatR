using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RemotiatR.Shared;

namespace RemotiatR.Server
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<JsonSerializer, JsonSerializer>();
            serviceCollection.TryAddSingleton<ISerializer,DefaultJsonSerializer>();
            return serviceCollection;
        }
    }
}
