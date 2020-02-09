using AutoMapper.QuickMaps.MappingMatchers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoMapper.QuickMaps.Configuration
{
    public static class IMapperConfigurationExpressionExtensions
    {
        public static IMapperConfigurationExpression CreateQuickMaps(
            this IMapperConfigurationExpression mapperConfigurationExpression,
            Action<ICreateQuickMapsConfigurationOptions> configure
        )
        {
            var options = new CreateQuickMapsConfigurationOptions();
            configure(options);
            
            if (!options.AssembliesToScan.Any())
                throw new InvalidOperationException($"{nameof(CreateQuickMaps)} must be configured with at least one {nameof(Assembly)}");
            if (!options.MappingMatchers.Any())
                throw new InvalidOperationException($"{nameof(CreateQuickMaps)} must be configured with at least one {nameof(MappingMatcher)}");

            var typesToMatch = options.AssembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && x.IsVisible && x.GetConstructors().Any(x => !x.IsStatic && x.IsPublic))
                .ToArray();

            var mappingMatchers = options.MappingMatchers
                .ToArray();

            foreach (var sourceType in typesToMatch)
                foreach(var destinationType in typesToMatch)
                    foreach(var mappingMatcher in mappingMatchers)
                        if (mappingMatcher(sourceType, destinationType))
                        {
                            mapperConfigurationExpression.CreateMap(sourceType.GetMappingType(), destinationType.GetMappingType());
                            break;
                        }

            
            return mapperConfigurationExpression;
        }

        private static Type GetMappingType(this Type type) =>
            typeof(IEnumerable).IsAssignableFrom(type)
                ? type.GetEnumerableType()
                : type;

        private static Type GetEnumerableType(this Type type) => 
            type.GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
    }
}
