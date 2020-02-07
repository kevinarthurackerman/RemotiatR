using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FluentValidation;

namespace RemotiatR.Server.FluentValidation
{
    public static class IServiceCollectionExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services, params Type[] assemblyTypeMarkers) =>
            AddFluentValidation(services, assemblyTypeMarkers, ServiceLifetime.Transient);

        public static void AddFluentValidation(this IServiceCollection services, params Assembly[] assemblies) =>
            AddFluentValidation(services, assemblies, ServiceLifetime.Transient);

        public static void AddFluentValidation(this IServiceCollection services, IEnumerable<Type> assemblyTypeMarkers, ServiceLifetime lifetime) =>
            AddFluentValidation(services, assemblyTypeMarkers.Select(x => x.Assembly), lifetime);

        public static void AddFluentValidation(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), lifetime));

            services.AddValidatorsFromAssemblies(assemblies, lifetime);
        }
    }
}
