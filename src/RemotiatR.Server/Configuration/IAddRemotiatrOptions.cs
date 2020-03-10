using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace RemotiatR.Server.Configuration
{
    public interface IAddRemotiatrOptions
    {
        IAddRemotiatrOptions SetMessageKeyGenerator(Func<Type, string> keyGenerator);

        IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan);

        IAddRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers);

        IAddRemotiatrOptions WithMediatorImplementationType(Type implementationType);

        IAddRemotiatrOptions WithMediatorLifetime(ServiceLifetime serviceLifetime);
    }
}
