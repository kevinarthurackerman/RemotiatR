using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;
using System;
using System.Reflection;

namespace RemotiatR.Client.Configuration
{
    public interface IAddRemotiatrOptions
    {
        IRemotiatrServiceCollection Services { get; }

        IAddRemotiatrOptions SetMessageKeyGenerator(Func<Type, string> keyGenerator);

        IAddRemotiatrOptions SetEndpointUri(Uri endpointUri);

        IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan);

        IAddRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers);

        IAddRemotiatrOptions WithMediatorImplementationType(Type implementationType);

        IAddRemotiatrOptions WithMediatorLifetime(ServiceLifetime serviceLifetime);
    }
}
