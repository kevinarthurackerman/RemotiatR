using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;
using System;
using System.Reflection;

namespace RemotiatR.Server
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions ConfigureMediator<TMediator>(this IAddRemotiatrOptions options, ServiceLifetime serviceLifetime) where TMediator : IMediator =>
            options.ConfigureMediator(typeof(TMediator), serviceLifetime);

        public static IAddRemotiatrOptions AddHost(this IAddRemotiatrOptions options, Uri rootUri, params Assembly[] assemblies) =>
            options.AddHost(rootUri, x => new Uri("remotiatr", UriKind.Relative), typeof(IMessageSerializer), assemblies);

        public static IAddRemotiatrOptions AddHost<TMessageSerializer>(
            this IAddRemotiatrOptions options,
            Uri rootUri,
            Func<Type, Uri> pathLocator,
            params Assembly[] assemblies
        )
            where TMessageSerializer : IMessageSerializer
        => options.AddHost(rootUri, pathLocator, typeof(TMessageSerializer), assemblies);
    }
}
