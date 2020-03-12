using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;

namespace RemotiatR.MessageTransport.Http.Server
{
    public static class IRemotiatrServiceCollectionExtensions
    {
        public static IRemotiatrServiceCollection AddHttpMessageTransport(this IRemotiatrServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return serviceCollection;
        }
    }
}
