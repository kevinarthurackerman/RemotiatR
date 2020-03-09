using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RemotiatR.Shared.Internal;

namespace RemotiatR.Server.Configuration
{
    internal class UseRemotiatrOptions : IUseRemotiatrOptions
    {
        internal Func<Type, Uri> UriBuilder { get; private set; } = RequestUriBuilder.DefaultUriBuilder;

        internal IEnumerable<Assembly> AssembliesToScan { get; private set; } = new Assembly[0];

        public IUseRemotiatrOptions SetUriBuilder(Func<Type, Uri> uriBuilder)
        {
            UriBuilder = uriBuilder ?? throw new ArgumentNullException(nameof(uriBuilder));
            return this;
        }

        public IUseRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan)
        {
            if (assembliesToScan == null) throw new ArgumentNullException(nameof(assembliesToScan));

            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }

        public IUseRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers)
        {
            if (assemblyTypeMarkers == null) throw new ArgumentNullException(nameof(assemblyTypeMarkers));

            var assembliesToScan = assemblyTypeMarkers.Select(x => x.Assembly).ToArray();
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }
    }
}
