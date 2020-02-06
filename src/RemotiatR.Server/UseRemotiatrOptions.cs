using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RemotiatR.Server
{
    public interface IUseRemotiatrOptions
    {
        IUseRemotiatrOptions SetUriBuilder(Func<Type, Uri> uriBuilder);

        IUseRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan);

        IUseRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers);
    }

    internal class UseRemotiatrOptions : IUseRemotiatrOptions
    {
        internal Func<Type, Uri> UriBuilder { get; private set; }

        internal IEnumerable<Assembly> AssembliesToScan { get; private set; } = new Assembly[0];

        public IUseRemotiatrOptions SetUriBuilder(Func<Type, Uri> uriBuilder)
        {
            UriBuilder = uriBuilder;
            return this;
        }

        public IUseRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan)
        {
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }

        public IUseRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers)
        {
            var assembliesToScan = assemblyTypeMarkers.Select(x => x.Assembly).ToArray();
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }
    }
}
