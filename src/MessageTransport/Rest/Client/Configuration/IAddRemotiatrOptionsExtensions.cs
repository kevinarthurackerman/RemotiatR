using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using System;

namespace RemotiatR.MessageTransport.Rest.Client
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions AddRestMessageTransport(this IAddRemotiatrOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.Services.TryAddScoped<IMessageTransport, DefaultRestMessageTransport>();

            return options;
        }
    }
}
