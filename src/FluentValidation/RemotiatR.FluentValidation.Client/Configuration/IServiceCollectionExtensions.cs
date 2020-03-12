using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RemotiatR.FluentValidation.Client
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
            serviceCollection.AddSingleton(new KeyMessageTypeMapping("FluentValidationErrors", typeof(ValidationError[])));

            serviceCollection.TryAddScoped<IValidationErrorsAccessor, DefaultValidationErrorsAccessor>();

            serviceCollection.Add(new ServiceDescriptor(
                typeof(IMessagePipelineHandler),
                x =>
                {
                    var validationErrorsAccessor = x.GetRequiredService<IValidationErrorsAccessor>();
                    return new ValidationMessagePipelineHandler(validationErrorsAccessor, x);
                },
                lifetime)
            );

            serviceCollection.AddValidatorsFromAssemblies(assemblies, lifetime);

            return serviceCollection;
        }
    }
}
