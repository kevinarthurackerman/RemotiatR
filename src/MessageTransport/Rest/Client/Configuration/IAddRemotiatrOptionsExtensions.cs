using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using RemotiatR.Shared;
using System;
using System.Reflection;
using RemotiatR.MessageTransport.Rest.Shared.Internal;

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

        public static IAddRemotiatrOptions AddRestHost(this IAddRemotiatrOptions options, Uri rootUri, params Assembly[] assemblies) =>
            options.AddHost(rootUri, Constants.PathLocator, typeof(IMessageSerializer), typeof(IMessageTransport), assemblies);
    }
}
