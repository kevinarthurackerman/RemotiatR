using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace RemotiatR.Client
{
    public interface IAddRemotiatrOptions
    {
        IServiceCollection Services { get; }

        IAddRemotiatrOptions SetRootUri(Uri rootUri);

        IAddRemotiatrOptions SetMessageUriLocator(Func<Type, Uri> messageUriLocator);

        IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan);

        IAddRemotiatrOptions WithMediatorImplementationType(Type implementationType);

        IAddRemotiatrOptions WithMediatorLifetime(ServiceLifetime serviceLifetime);
    }
}
