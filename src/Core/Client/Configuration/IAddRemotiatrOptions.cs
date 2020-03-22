using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace RemotiatR.Client
{
    public interface IAddRemotiatrOptions
    {
        IServiceCollection Services { get; }

        IAddRemotiatrOptions ConfigureMediator(Type implementationType, ServiceLifetime serviceLifetime);

        IAddRemotiatrOptions AddHost(Uri rootUri, Func<Type, Uri> pathLocator, Type messageSerializerType, Type messageTransportType, params Assembly[] assemblies);
    }
}
