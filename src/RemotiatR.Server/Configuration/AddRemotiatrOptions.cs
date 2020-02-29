using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RemotiatR.Client.Configuration
{
    internal class AddRemotiatrOptions : IAddRemotiatrOptions
    {
        internal IEnumerable<Assembly> AssembliesToScan { get; private set; } = new Assembly[0];

        internal Type MediatorImplementationType { get; private set; } = typeof(Mediator);

        internal ServiceLifetime MediatorServiceLifetime { get; private set; } = ServiceLifetime.Transient;

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

        public IAddRemotiatrOptions WithMediatorImplementationType(Type implementationType)
        {
            MediatorImplementationType = implementationType;
            return this;
        }

        public IAddRemotiatrOptions WithMediatorLifetime(ServiceLifetime serviceLifetime)
        {
            MediatorServiceLifetime = serviceLifetime;
            return this;
        }
    }
}
