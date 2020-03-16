using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

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

        public static IAddRemotiatrOptions SetDefaultRootUri(this IAddRemotiatrOptions addRemotiatrOptions, Uri authority)
        {
            if (addRemotiatrOptions == null) throw new ArgumentNullException(nameof(addRemotiatrOptions));
            if (!authority.IsAbsoluteUri) throw new InvalidOperationException($"{nameof(authority)} must be absolute");

            var uri = new Uri(authority, "remotiatr");

            return addRemotiatrOptions.SetRootUri(uri);
        }

        public static IAddRemotiatrOptions AddAssemblies(this IAddRemotiatrOptions addRemotiatrOptions, params Type[] assemblyTypeMarkers)
        {
            if (addRemotiatrOptions == null) throw new ArgumentNullException(nameof(addRemotiatrOptions));
            if (assemblyTypeMarkers == null) throw new ArgumentNullException(nameof(assemblyTypeMarkers));

            return addRemotiatrOptions.AddAssemblies(assemblyTypeMarkers.Select(x => x.Assembly).ToArray());
        }
    }
}
