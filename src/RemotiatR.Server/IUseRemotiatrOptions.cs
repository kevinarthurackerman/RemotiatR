using System;
using System.Reflection;

namespace RemotiatR.Server
{
    public interface IUseRemotiatrOptions
    {
        IUseRemotiatrOptions SetUriBuilder(Func<Type, Uri> uriBuilder);

        IUseRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan);

        IUseRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers);
    }
}
