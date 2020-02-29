using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace RemotiatR.Server.Configuration
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions WithMediatorImplementationType<TMediator>(this IAddRemotiatrOptions addRemotiatrOptions)
            where TMediator : IMediator
        {
            addRemotiatrOptions.WithMediatorImplementationType(typeof(TMediator));
            return addRemotiatrOptions;
        }

        public static IAddRemotiatrOptions AsSingleton(this IAddRemotiatrOptions addRemotiatrOptions) =>
            addRemotiatrOptions.WithMediatorLifetime(ServiceLifetime.Singleton);

        public static IAddRemotiatrOptions AsScoped(this IAddRemotiatrOptions addRemotiatrOptions) =>
            addRemotiatrOptions.WithMediatorLifetime(ServiceLifetime.Scoped);

        public static IAddRemotiatrOptions AsTransient(this IAddRemotiatrOptions addRemotiatrOptions) =>
            addRemotiatrOptions.WithMediatorLifetime(ServiceLifetime.Transient);
    }
}
