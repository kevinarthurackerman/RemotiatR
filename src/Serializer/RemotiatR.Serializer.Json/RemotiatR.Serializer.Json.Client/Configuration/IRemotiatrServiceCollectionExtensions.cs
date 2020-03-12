using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;
using RemotiatR.Serializer.Json.Shared;

namespace RemotiatR.Serializer.Json.Client
{
    public static class IRemotiatrServiceCollectionExtensions
    {
        public static IRemotiatrServiceCollection AddJsonSerializer(this IRemotiatrServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAddSingleton<IMessageSerializer, DefaultJsonMessageSerializer>();

            return serviceCollection;
        }
    }
}
