using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using RemotiatR.Shared;
using System;

namespace RemotiatR.MessageTransport.Http.Client
{
    public static class IRemotiatrServiceCollectionExtensions
    {
        public static IRemotiatrServiceCollection AddHttpMessageTransport(this IRemotiatrServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAddSingleton<IMessageTransport, DefaultHttpMessageTransport>();

            return serviceCollection;
        }
    }
}
