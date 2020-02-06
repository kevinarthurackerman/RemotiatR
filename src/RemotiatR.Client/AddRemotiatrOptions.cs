using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RemotiatR.Client
{
    public interface IAddRemotiatrOptions
    {
        IAddRemotiatrOptions AddServer<TServerMarker>(Action<IAddServerOptions> configure = null)
            where TServerMarker : IRemotiatr;

        IAddRemotiatrOptions AddDefaultServer(Action<IAddServerOptions> configure = null) => AddServer<IRemotiatr>(configure);
    }

    internal class AddRemotiatrOptions : IAddRemotiatrOptions
    {
        internal IDictionary<Type, AddServerOptions> ServerConfigurations = new Dictionary<Type, AddServerOptions>();

        public IAddRemotiatrOptions AddServer<TServerMarker>(Action<IAddServerOptions> configure = null)
            where TServerMarker : IRemotiatr
        {
            var options = new AddServerOptions();
            configure?.Invoke(options);

            if (!ServerConfigurations.TryAdd(typeof(TServerMarker), options))
                throw new InvalidOperationException($"A server of type {typeof(TServerMarker).FullName} has already been registered.");

            return this;
        }

        public IAddRemotiatrOptions AddDefaultServer(Action<IAddServerOptions> configure = null) => AddServer<IRemotiatr>(configure);
    }

    public interface IAddServerOptions
    {
        IAddServerOptions SetMessageSenderLocator(Func<IServiceProvider, IMessageSender> messageSenderLocator);

        IAddServerOptions SetUriBuilder(Func<Type, Uri> uriBuilder);

        IAddServerOptions SetBaseUri(Uri baseUri);

        IAddServerOptions AddAssemblies(params Assembly[] assembliesToScan);

        IAddServerOptions AddAssemblies(params Type[] assemblyTypeMarkers);
    }

    internal class AddServerOptions : IAddServerOptions
    {
        internal Func<IServiceProvider, IMessageSender> MessageSenderLocator { get; private set; } = x => x.GetRequiredService<IMessageSender>();

        internal Func<Type, Uri> UriBuilder { get; private set; } = x => new Uri("/api/" + x.FullName.Split('.').Last().Replace('+', '/'), UriKind.Relative);

        internal Uri BaseUri { get; private set; }

        internal IEnumerable<Assembly> AssembliesToScan { get; private set; } = new Assembly[0];

        public IAddServerOptions SetMessageSenderLocator(Func<IServiceProvider, IMessageSender> messageSenderLocator)
        {
            MessageSenderLocator = messageSenderLocator;
            return this;
        }

        public IAddServerOptions SetUriBuilder(Func<Type, Uri> uriBuilder)
        {
            UriBuilder = uriBuilder;
            return this;
        }

        public IAddServerOptions SetBaseUri(Uri baseUri)
        {
            if (!baseUri.IsAbsoluteUri) throw new ArgumentException("Base URI must be absolute");

            BaseUri = baseUri;
            return this;
        }

        public IAddServerOptions AddAssemblies(params Assembly[] assembliesToScan)
        {
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }

        public IAddServerOptions AddAssemblies(params Type[] assemblyTypeMarkers)
        {
            var assembliesToScan = assemblyTypeMarkers.Select(x => x.Assembly).ToArray();
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }
    }
}
