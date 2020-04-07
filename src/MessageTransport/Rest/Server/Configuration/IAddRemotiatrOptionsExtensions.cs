using RemotiatR.Server;
using RemotiatR.Shared;
using System;
using System.Reflection;

namespace RemotiatR.MessageTransport.Rest.Server
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions AddHost(this IAddRemotiatrOptions options, Uri rootUri, params Assembly[] assemblies) =>
            options.AddHost(rootUri, x => new Uri(x.FullName.Replace('.','/').Replace('+','-'), UriKind.Relative), typeof(IMessageSerializer), assemblies);
    }
}
