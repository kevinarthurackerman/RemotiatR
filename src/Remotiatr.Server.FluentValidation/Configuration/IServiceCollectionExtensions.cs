using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared.Internal;
using RemotiatR.Shared;
using System.ComponentModel;

namespace RemotiatR.Server.FluentValidation.Configuration
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection serviceCollection, params Type[] assemblyTypeMarkers)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (assemblyTypeMarkers == null) throw new ArgumentNullException(nameof(assemblyTypeMarkers));

            return AddFluentValidation(serviceCollection, assemblyTypeMarkers, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddFluentValidation(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            return AddFluentValidation(serviceCollection, assemblies, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddFluentValidation(this IServiceCollection serviceCollection, IEnumerable<Type> assemblyTypeMarkers, ServiceLifetime serviceLifetime)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (assemblyTypeMarkers == null) throw new ArgumentNullException(nameof(assemblyTypeMarkers));
            if (!Enum.IsDefined(typeof(ServiceLifetime), serviceLifetime)) throw new InvalidEnumArgumentException(nameof(serviceLifetime), (int)serviceLifetime, typeof(ServiceLifetime));

            return AddFluentValidation(serviceCollection, assemblyTypeMarkers.Select(x => x.Assembly), serviceLifetime);
        }

        public static IServiceCollection AddFluentValidation(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies, ServiceLifetime serviceLifetime)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            if (!Enum.IsDefined(typeof(ServiceLifetime), serviceLifetime)) throw new InvalidEnumArgumentException(nameof(serviceLifetime), (int)serviceLifetime, typeof(ServiceLifetime));

            serviceCollection.TryAddSingleton<ISerializer, DefaultJsonSerializer>();

            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            serviceCollection.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), serviceLifetime));

            serviceCollection.AddValidatorsFromAssemblies(assemblies, serviceLifetime);

            return serviceCollection;
        }
    }
}
