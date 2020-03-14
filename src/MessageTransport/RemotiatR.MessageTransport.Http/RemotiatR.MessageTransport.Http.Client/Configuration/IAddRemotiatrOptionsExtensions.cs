using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using System;

namespace RemotiatR.MessageTransport.Http.Client
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions AddHttpMessageTransport(this IAddRemotiatrOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.Services.TryAddTransient<IMessageTransport, DefaultHttpMessageTransport>();

            return options;
        }
    }
}
