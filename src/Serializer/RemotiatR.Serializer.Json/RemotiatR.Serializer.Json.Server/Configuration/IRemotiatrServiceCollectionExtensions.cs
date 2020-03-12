using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;

namespace RemotiatR.Serializer.Json.Server.Configuration
{
    public static class IRemotiatrServiceCollectionExtensions
    {
        private static IRemotiatrServiceCollection AddJsonSerializer(this IRemotiatrServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAddSingleton<IMessageSerializer, DefaultJsonMessageSerializer>();

            return serviceCollection;
        }
    }
}
