using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace RemotiatR.Client.Configuration
{
    public interface IAddRemotiatrOptions
    {
        IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan);

        IAddRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers);

        IAddRemotiatrOptions WithMediatorImplementationType(Type implementationType);

        IAddRemotiatrOptions WithMediatorLifetime(ServiceLifetime serviceLifetime);
    }
}
