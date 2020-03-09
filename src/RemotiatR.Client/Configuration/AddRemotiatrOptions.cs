using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using RemotiatR.Shared.Internal;

namespace RemotiatR.Client.Configuration
{
    internal class AddRemotiatrOptions : IAddRemotiatrOptions
    {
        public IServiceCollection Services { get; } = new ServiceCollection();

        internal Func<Type, Uri> UriBuilder { get; private set; } = RequestUriBuilder.DefaultUriBuilder;

        internal Uri? BaseUri { get; private set; }

        internal IEnumerable<Assembly> AssembliesToScan { get; private set; } = new Assembly[0];

        internal Type MediatorImplementationType { get; private set; } = typeof(Mediator);

        internal ServiceLifetime MediatorServiceLifetime { get; private set; } = ServiceLifetime.Transient;

        public IAddRemotiatrOptions SetUriBuilder(Func<Type, Uri> uriBuilder)
        {
            UriBuilder = uriBuilder ?? throw new ArgumentNullException(nameof(uriBuilder));
            return this;
        }

        public IAddRemotiatrOptions SetBaseUri(Uri baseUri)
        {
            if (baseUri == null) throw new ArgumentNullException(nameof(baseUri));
            if (!baseUri.IsAbsoluteUri) throw new ArgumentException("Base URI must be absolute");

            BaseUri = baseUri;
            return this;
        }

        public IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan)
        {
            if (assembliesToScan == null) throw new ArgumentNullException(nameof(assembliesToScan));

            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }

        public IAddRemotiatrOptions AddAssemblies(params Type[] assemblyTypeMarkers)
        {
            if (assemblyTypeMarkers == null) throw new ArgumentNullException(nameof(assemblyTypeMarkers));

            var assembliesToScan = assemblyTypeMarkers.Select(x => x.Assembly).ToArray();
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }

        public IAddRemotiatrOptions WithMediatorImplementationType(Type implementationType)
        {
            MediatorImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            return this;
        }

        public IAddRemotiatrOptions WithMediatorLifetime(ServiceLifetime serviceLifetime)
        {
            if (!Enum.IsDefined(typeof(ServiceLifetime), serviceLifetime)) throw new InvalidEnumArgumentException(nameof(serviceLifetime), (int)serviceLifetime, typeof(ServiceLifetime));

            MediatorServiceLifetime = serviceLifetime;
            return this;
        }
    }
}
