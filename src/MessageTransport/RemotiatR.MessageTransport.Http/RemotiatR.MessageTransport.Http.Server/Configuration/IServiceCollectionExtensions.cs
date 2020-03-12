using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;

namespace RemotiatR.MessageTransport.Http.Server.Configuration
{
    public static class IServiceCollectionExtensions
    {
        private static IRemotiatrServiceCollection AddHttp<TMarker, TRemotiatr>(this IRemotiatrServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return serviceCollection;
        }
    }
}
