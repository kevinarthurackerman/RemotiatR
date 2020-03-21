using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RemotiatR.FluentValidation.Client
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions AddFluentValidation(this IAddRemotiatrOptions options, params Type[] assemblyTypeMarkers)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (assemblyTypeMarkers == null) throw new ArgumentNullException(nameof(assemblyTypeMarkers));

            Configure(options, assemblyTypeMarkers.Select(x => x.Assembly), ServiceLifetime.Transient);

            return options;
        }

        public static IAddRemotiatrOptions AddFluentValidation(this IAddRemotiatrOptions options, params Assembly[] assemblies)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            Configure(options, assemblies, ServiceLifetime.Transient);

            return options;
        }

        public static IAddRemotiatrOptions AddFluentValidation(this IAddRemotiatrOptions options, IEnumerable<Type> assemblyTypeMarkers, ServiceLifetime serviceLifetime)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (assemblyTypeMarkers == null) throw new ArgumentNullException(nameof(assemblyTypeMarkers));
            if (!Enum.IsDefined(typeof(ServiceLifetime), serviceLifetime)) throw new InvalidEnumArgumentException(nameof(serviceLifetime), (int)serviceLifetime, typeof(ServiceLifetime));

            Configure(options, assemblyTypeMarkers.Select(x => x.Assembly), serviceLifetime);

            return options;
        }

        public static IAddRemotiatrOptions AddFluentValidation(this IAddRemotiatrOptions options, IEnumerable<Assembly> assemblies, ServiceLifetime serviceLifetime)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            if (!Enum.IsDefined(typeof(ServiceLifetime), serviceLifetime)) throw new InvalidEnumArgumentException(nameof(serviceLifetime), (int)serviceLifetime, typeof(ServiceLifetime));

            Configure(options, assemblies, serviceLifetime);

            return options;
        }

        private static void Configure(IAddRemotiatrOptions options, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime)
        { 
            options.Services.TryAddScoped<IValidationErrorsAccessor, DefaultValidationErrorsAccessor>();

            options.Services.Add(new ServiceDescriptor(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>),
                lifetime)
            );

            options.Services.AddValidatorsFromAssemblies(assemblies, lifetime);
        }
    }
}
