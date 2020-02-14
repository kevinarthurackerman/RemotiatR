using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client.MessageSenders;
using RemotiatR.Shared.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RemotiatR.Client.FluentValidation.Configuration
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

            serviceCollection.TryAddScoped<IValidationErrorsAccessor, DefaultValidationErrorsAccessor>();

            serviceCollection.AddSingleton<IHttpMessageHandler, ValidationHttpMessageHandler>();

            serviceCollection.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), lifetime));

            serviceCollection.AddValidatorsFromAssemblies(assemblies, lifetime);

            return serviceCollection;
        }
    }
}
