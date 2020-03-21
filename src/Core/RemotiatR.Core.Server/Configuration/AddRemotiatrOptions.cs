using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using RemotiatR.Shared;
using MediatR.Registration;

namespace RemotiatR.Server
{
    internal class AddRemotiatrOptions : IAddRemotiatrOptions
    {
        private readonly HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();

        public IServiceCollection Services { get; } = new ServiceCollection();

        public IAddRemotiatrOptions ConfigureMediator(Type implementationType, ServiceLifetime serviceLifetime)
        {
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));
            if (!typeof(IMediator).IsAssignableFrom(implementationType)) 
                throw new ArgumentException($"{nameof(implementationType)} must be assignable to {typeof(IMediator).FullName}");
            if (!Enum.IsDefined(typeof(ServiceLifetime), serviceLifetime))
                throw new InvalidEnumArgumentException(nameof(serviceLifetime), (int)serviceLifetime, typeof(ServiceLifetime));
            
            var registeredIMediators = Services.Where(x => x.ServiceType == typeof(IMediator));
            foreach (var registeredIMediator in registeredIMediators)
                Services.Remove(registeredIMediator);

            Services.Add(new ServiceDescriptor(typeof(IMediator), implementationType, serviceLifetime));

            return this;
        }

        public IAddRemotiatrOptions AddHost(Uri rootUri, Func<Type, Uri> pathLocator, Type messageSerializerType, params Assembly[] assemblies)
        {
            if (rootUri == null) throw new ArgumentNullException(nameof(rootUri));
            if (!rootUri.IsAbsoluteUri) throw new ArgumentException($"{nameof(rootUri)} must be absolute");
            if (rootUri != new Uri($"{rootUri.Scheme}://{rootUri.Authority}")) throw new ArgumentException($"{nameof(rootUri)} must only contain URI scheme and authority");
            if (pathLocator == null) throw new ArgumentNullException(nameof(pathLocator));
            if (messageSerializerType == null) throw new ArgumentNullException(nameof(messageSerializerType));
            if (!typeof(IMessageSerializer).IsAssignableFrom(messageSerializerType))
                throw new ArgumentException($"{nameof(messageSerializerType)} must be assignable to {typeof(IMessageSerializer).FullName}");
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            assemblies = assemblies.Distinct().ToArray();

            foreach(var assembly in assemblies)
                if (!_registeredAssemblies.Add(assembly))
                    throw new InvalidOperationException($"Assembly {assembly.FullName} was previously registered. Assemblies must only be registered once");

            var notificationTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsNotificationType())
                .ToArray();

            var requestTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsRequestType())
                .ToArray();

            var messageInfos = new List<MessageInfo>();
            foreach (var messageType in notificationTypes.Concat(requestTypes))
            {
                var pathUri = pathLocator(messageType);
                if (pathUri == null) throw new InvalidOperationException($"{nameof(pathLocator)} produced invalid null path");
                if (pathUri.IsAbsoluteUri) throw new InvalidOperationException($"{nameof(pathLocator)} must only provide relative URIs. Path '{pathUri.ToString()}' is absolute");

                messageInfos.Add(new MessageInfo(messageType, pathUri));
            }

            var messageHostInfo = new MessageHostInfo(rootUri, messageSerializerType, messageInfos);

            foreach (var messageInfo in messageInfos) messageInfo.SetHost(messageHostInfo);

            Services.AddSingleton<IMessageHostInfo>(messageHostInfo);

            ServiceRegistrar.AddMediatRClasses(Services, assemblies);

            return this;
        }
    }
}
