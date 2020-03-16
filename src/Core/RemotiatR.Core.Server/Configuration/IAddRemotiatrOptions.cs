using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;
using System;
using System.Reflection;

namespace RemotiatR.Server
{
    public interface IAddRemotiatrOptions
    {
        IServiceCollection Services { get; }

        IAddRemotiatrOptions SetMessageUriLocator(Func<Type, Uri> messageUriLocator);

        IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan);

        IAddRemotiatrOptions WithMediatorImplementationType(Type implementationType);

        IAddRemotiatrOptions WithMediatorLifetime(ServiceLifetime serviceLifetime);
    }
}
