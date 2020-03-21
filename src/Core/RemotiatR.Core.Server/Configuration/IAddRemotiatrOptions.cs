using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace RemotiatR.Server
{
    public interface IAddRemotiatrOptions
    {
        IServiceCollection Services { get; }

        IAddRemotiatrOptions ConfigureMediator(Type implementationType, ServiceLifetime serviceLifetime);

        IAddRemotiatrOptions AddHost(Uri rootUri, Func<Type, Uri> pathLocator, Type messageSerializerType, params Assembly[] assemblies);
    }
}
