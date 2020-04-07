using RemotiatR.Server;
using RemotiatR.Shared;
using System;
using System.Reflection;
using RemotiatR.MessageTransport.Rest.Shared.Internal;

namespace RemotiatR.MessageTransport.Rest.Server
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions AddHost(this IAddRemotiatrOptions options, Uri rootUri, params Assembly[] assemblies) =>
            options.AddHost(rootUri, Constants.PathLocator, typeof(IMessageSerializer), assemblies);
    }
}
