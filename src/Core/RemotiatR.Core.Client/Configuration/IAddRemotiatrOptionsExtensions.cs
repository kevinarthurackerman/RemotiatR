using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace RemotiatR.Client
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions WithMediatorImplementationType<TMediator>(this IAddRemotiatrOptions addRemotiatrOptions)
            where TMediator : IMediator =>
            addRemotiatrOptions?.WithMediatorImplementationType(typeof(TMediator)) ?? throw new ArgumentNullException(nameof(addRemotiatrOptions));

        public static IAddRemotiatrOptions AsSingleton(this IAddRemotiatrOptions addRemotiatrOptions) =>
            addRemotiatrOptions?.WithMediatorLifetime(ServiceLifetime.Singleton) ?? throw new ArgumentNullException(nameof(addRemotiatrOptions));

        public static IAddRemotiatrOptions AsScoped(this IAddRemotiatrOptions addRemotiatrOptions) =>
            addRemotiatrOptions?.WithMediatorLifetime(ServiceLifetime.Scoped) ?? throw new ArgumentNullException(nameof(addRemotiatrOptions));

        public static IAddRemotiatrOptions AsTransient(this IAddRemotiatrOptions addRemotiatrOptions) =>
            addRemotiatrOptions?.WithMediatorLifetime(ServiceLifetime.Transient) ?? throw new ArgumentNullException(nameof(addRemotiatrOptions));

        public static IAddRemotiatrOptions SetDefaultEndpointUriWithRoot(this IAddRemotiatrOptions addRemotiatrOptions, Uri endpointRoot)
        {
            if (addRemotiatrOptions == null) throw new ArgumentNullException(nameof(addRemotiatrOptions));
            if (!endpointRoot.IsAbsoluteUri) throw new InvalidOperationException($"{nameof(endpointRoot)} must be absolute");

            var uri = new Uri(endpointRoot, "remotiatr");

            return addRemotiatrOptions.SetEndpointUri(uri);
        }
    }
}
