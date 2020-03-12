using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using RemotiatR.Shared;
using System;

namespace RemotiatR.MessageTransport.Http.Client.Configuration
{
    public static class IServiceCollectionExtensions
    {
        public static IRemotiatrServiceCollection AddHttp(this IRemotiatrServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAddSingleton<IMessageTransport, DefaultHttpMessageTransport>();

            // todo: remove
            //serviceCollection.TryAddSingleton(x =>
            //{
            //    var httpClient = new HttpClient();
            //    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            //    return httpClient;
            //});

            return serviceCollection;
        }
    }
}
