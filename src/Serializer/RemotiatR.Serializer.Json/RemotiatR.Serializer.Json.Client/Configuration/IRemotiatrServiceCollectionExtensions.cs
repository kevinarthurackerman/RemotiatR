using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using RemotiatR.Shared;
using RemotiatR.Serializer.Json.Shared;
using System;

namespace RemotiatR.Serializer.Json.Client
{
    public static class IRemotiatrServiceCollectionExtensions
    {
        public static IAddRemotiatrOptions AddJsonSerializer(this IAddRemotiatrOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.Services.TryAddScoped<IMessageSerializer, DefaultJsonMessageSerializer>();

            return options;
        }
    }
}
