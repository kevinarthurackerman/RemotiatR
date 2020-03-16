using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;
using RemotiatR.Server;
using MediatR;

namespace RemotiatR.FluentValidation.Server
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

        private static IAddRemotiatrOptions Configure(IAddRemotiatrOptions options, IEnumerable<Assembly> assemblies, ServiceLifetime serviceLifetime)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            if (!Enum.IsDefined(typeof(ServiceLifetime), serviceLifetime)) throw new InvalidEnumArgumentException(nameof(serviceLifetime), (int)serviceLifetime, typeof(ServiceLifetime));

            options.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            options.Services.Add(new ServiceDescriptor(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>),
                serviceLifetime
            ));

            options.Services.AddValidatorsFromAssemblies(assemblies, serviceLifetime);

            return options;
        }
    }
}
