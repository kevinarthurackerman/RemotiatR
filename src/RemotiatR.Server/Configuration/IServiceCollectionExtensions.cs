using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using RemotiatR.Shared;
using RemotiatR.Shared.Internal;

namespace RemotiatR.Server.Configuration
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotiatr(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<JsonSerializer, JsonSerializer>();

            serviceCollection.TryAddSingleton<ISerializer,DefaultJsonSerializer>();

            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return serviceCollection;
        }
    }
}
