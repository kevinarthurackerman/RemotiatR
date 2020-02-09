using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RemotiatR.Client.Configuration
{
    internal class AddRemotiatrOptions : IAddRemotiatrOptions
    {
        public IServiceCollection Services { get; } = new ServiceCollection();

        internal Func<Type, Uri> UriBuilder { get; private set; } = x => new Uri("/api/" + x.FullName.Split('.').Last().Replace('+', '/'), UriKind.Relative);

        internal Uri BaseUri { get; private set; }

        internal IEnumerable<Assembly> AssembliesToScan { get; private set; } = new Assembly[0];

        public IAddRemotiatrOptions SetUriBuilder(Func<Type, Uri> uriBuilder)
        {
            UriBuilder = uriBuilder;
            return this;
        }

        public IAddRemotiatrOptions SetBaseUri(Uri baseUri)
        {
            if (!baseUri.IsAbsoluteUri) throw new ArgumentException("Base URI must be absolute");

            BaseUri = baseUri;
            return this;
        }

        public IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan)
        {
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }

        public IAddRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers)
        {
            var assembliesToScan = assemblyTypeMarkers.Select(x => x.Assembly).ToArray();
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }
    }
}
