using AutoMapper.QuickMaps.MappingMatchers;
using System;
using System.Reflection;

namespace AutoMapper.QuickMaps.Configuration
{
    public interface ICreateQuickMapsConfigurationOptions
    {
        ICreateQuickMapsConfigurationOptions AddAssemblies(params Assembly[] assemblies);

        ICreateQuickMapsConfigurationOptions AddAssemblies(params Type[] assemblyTypeMarkers);

        ICreateQuickMapsConfigurationOptions AddMappingMatchers(params MappingMatcher[] mappingMatchers);
    }
}
