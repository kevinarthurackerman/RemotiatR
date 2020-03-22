using RemotiatR.Server;
using RemotiatR.Shared;
using RemotiatR.Serializer.Json.Shared;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RemotiatR.Serializer.Json.Server
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions AddJsonSerializer(this IAddRemotiatrOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.Services.TryAddScoped<IMessageSerializer, DefaultJsonMessageSerializer>();

            return options;
        }
    }
}
