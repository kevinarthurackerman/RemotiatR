using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FluentValidation;
using RemotiatR.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared.Internal;

namespace RemotiatR.Server.FluentValidation.Configuration
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection serviceCollection, params Type[] assemblyTypeMarkers) =>
            AddFluentValidation(serviceCollection, assemblyTypeMarkers, ServiceLifetime.Transient);

        public static IServiceCollection AddFluentValidation(this IServiceCollection serviceCollection, params Assembly[] assemblies) =>
            AddFluentValidation(serviceCollection, assemblies, ServiceLifetime.Transient);

        public static IServiceCollection AddFluentValidation(this IServiceCollection serviceCollection, IEnumerable<Type> assemblyTypeMarkers, ServiceLifetime lifetime) =>
            AddFluentValidation(serviceCollection, assemblyTypeMarkers.Select(x => x.Assembly), lifetime);

        public static IServiceCollection AddFluentValidation(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime)
        {
            serviceCollection.TryAddSingleton<ISerializer, DefaultJsonSerializer>();

            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            serviceCollection.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), lifetime));

            serviceCollection.AddValidatorsFromAssemblies(assemblies, lifetime);

            return serviceCollection;
        }
    }
}
