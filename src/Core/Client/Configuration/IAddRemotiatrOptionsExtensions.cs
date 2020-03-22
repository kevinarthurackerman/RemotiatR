using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using RemotiatR.Shared;

namespace RemotiatR.Client
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions ConfigureMediator<TMediator>(this IAddRemotiatrOptions options, ServiceLifetime serviceLifetime) where TMediator : IMediator =>
            options.ConfigureMediator(typeof(TMediator), serviceLifetime);

        public static IAddRemotiatrOptions AddHost(this IAddRemotiatrOptions options, Uri rootUri, params Assembly[] assemblies) =>
            options.AddHost(rootUri, x => new Uri("remotiatr", UriKind.Relative), typeof(IMessageSerializer), typeof(IMessageTransport), assemblies);

        public static IAddRemotiatrOptions AddHost<TMessageSerializer, TMessageTransport>(
            this IAddRemotiatrOptions options,
            Uri rootUri,
            Func<Type, Uri> pathLocator,
            params Assembly[] assemblies
        ) 
            where TMessageSerializer : IMessageSerializer
            where TMessageTransport : IMessageTransport 
        => options.AddHost(rootUri, pathLocator, typeof(TMessageSerializer), typeof(TMessageTransport), assemblies);
    }
}
