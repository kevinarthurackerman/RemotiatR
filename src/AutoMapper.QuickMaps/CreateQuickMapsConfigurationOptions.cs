using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoMapper.QuickMaps
{
    public interface ICreateQuickMapsConfigurationOptions
    {
        ICreateQuickMapsConfigurationOptions AddAssemblies(params Assembly[] assemblies);

        ICreateQuickMapsConfigurationOptions AddAssemblyTypeMarkers(params Type[] assemblyTypeMarkers);

        ICreateQuickMapsConfigurationOptions AddMappingMatchers(params MappingMatcher[] mappingMatchers);
    }

    internal class CreateQuickMapsConfigurationOptions : ICreateQuickMapsConfigurationOptions
    {
        internal IEnumerable<Assembly> Assemblies { get; set; }

        internal IEnumerable<MappingMatcher> MappingMatchers { get; set; }

        public ICreateQuickMapsConfigurationOptions AddAssemblies(params Assembly[] assemblies)
        {
            Assemblies = Assemblies == null
                ? assemblies
                : Assemblies.Concat(assemblies).ToArray();

            return this;
        }

        public ICreateQuickMapsConfigurationOptions AddAssemblyTypeMarkers(params Type[] assemblyTypeMarkers)
        {
            var assemblies = assemblyTypeMarkers.Select(x => x.Assembly).ToArray();

            Assemblies = Assemblies == null
                ? assemblies
                : Assemblies.Concat(assemblies).ToArray();

            return this;
        }

        public ICreateQuickMapsConfigurationOptions AddMappingMatchers(params MappingMatcher[] mappingMatchers)
        {
            MappingMatchers = MappingMatchers == null
                ? mappingMatchers
                : MappingMatchers.Concat(mappingMatchers).ToArray();

            return this;
        }
    }
}
