using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RemotiatR.Server
{
    internal class AddRemotiatrOptions : IAddRemotiatrOptions
    {
        public IServiceCollection Services { get; } = new ServiceCollection();

        internal Uri? RootUri { get; private set; }

        internal Func<Type, Uri> MessageUriLocator { get; private set; } = Constants.DefaultMessageUriLocator;

        internal IEnumerable<Assembly> AssembliesToScan { get; private set; } = new Assembly[0];

        internal Type MediatorImplementationType { get; private set; } = typeof(Mediator);

        internal ServiceLifetime MediatorServiceLifetime { get; private set; } = ServiceLifetime.Transient;

        public IAddRemotiatrOptions SetRootUri(Uri rootUri)
        {
            RootUri = rootUri ?? throw new ArgumentNullException(nameof(rootUri));
            if (!rootUri.IsAbsoluteUri) throw new ArgumentException($"{nameof(rootUri)} must be absolute.");

            return this;
        }

        public IAddRemotiatrOptions SetMessageUriLocator(Func<Type, Uri> messageUriLocator)
        {
            MessageUriLocator = messageUriLocator ?? throw new ArgumentNullException(nameof(messageUriLocator));

            return this;
        }

        public IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan)
        {
            if (assembliesToScan == null) throw new ArgumentNullException(nameof(assembliesToScan));

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
