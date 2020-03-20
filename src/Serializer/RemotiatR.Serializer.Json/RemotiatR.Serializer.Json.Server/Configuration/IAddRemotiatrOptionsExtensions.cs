using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;
using RemotiatR.Serializer.Json.Shared;
using RemotiatR.Server;

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
