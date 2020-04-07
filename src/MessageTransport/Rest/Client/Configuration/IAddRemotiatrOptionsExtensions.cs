using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using RemotiatR.Shared;
using System;
using System.Reflection;

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
            options.AddHost(rootUri, x => new Uri(x.FullName.Replace('.','/').Replace('+','-'), UriKind.Relative), typeof(IMessageSerializer), typeof(IMessageTransport), assemblies);
    }
}
