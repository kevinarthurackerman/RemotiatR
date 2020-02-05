using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RemotiatR.Client
{
    public interface IAddRemotiatrOptions
    {
        IAddRemotiatrOptions AddServer<TServerMarker>(UrlBuilder urlBuilder, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TServerMarker : IRemotiatr;

        IAddRemotiatrOptions AddDefaultServer(UrlBuilder urlBUilder, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped);
    }

    internal class AddRemotiatrOptions : IAddRemotiatrOptions
    {
        internal IDictionary<Type, ServerConfiguration> ServerConfigurations = new Dictionary<Type, ServerConfiguration>();

        public IAddRemotiatrOptions AddServer<TServerMarker>(UrlBuilder urlBuilder, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TServerMarker : IRemotiatr
        {
            if (!ServerConfigurations.TryAdd(typeof(TServerMarker), new ServerConfiguration(urlBuilder, serviceLifetime)))
                throw new InvalidOperationException($"A server of type {typeof(TServerMarker).FullName} has already been registered.");

            return this;
        }

        public IAddRemotiatrOptions AddDefaultServer(UrlBuilder urlBuilder, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) => AddServer<IRemotiatr>(urlBuilder, serviceLifetime);
    }

    internal class ServerConfiguration
    {
        internal UrlBuilder UrlBuilder { get; }
        internal ServiceLifetime ServiceLifetime { get; }

        internal ServerConfiguration(UrlBuilder urlBuilder, ServiceLifetime serviceLifetime)
        {
            UrlBuilder = urlBuilder;
            ServiceLifetime = serviceLifetime;
        }
    }
    
    public delegate string UrlBuilder(Type request);
}
