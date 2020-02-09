using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoMapper.QuickMaps
{
    internal class CreateQuickMapsConfigurationOptions : ICreateQuickMapsConfigurationOptions
    {
        internal IEnumerable<Assembly> AssembliesToScan { get; private set; } = new Assembly[0];

        internal IEnumerable<MappingMatcher> MappingMatchers { get; private set; } = new MappingMatcher[0];

        public ICreateQuickMapsConfigurationOptions AddAssemblies(params Assembly[] assembliesToScan)
        {
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }

        public ICreateQuickMapsConfigurationOptions AddAssemblies(params Type[] assemblyTypeMarkers)
        {
            var assembliesToScan = assemblyTypeMarkers.Select(x => x.Assembly).ToArray();
            AssembliesToScan = AssembliesToScan.Concat(assembliesToScan).Distinct().ToArray();
            return this;
        }

        public ICreateQuickMapsConfigurationOptions AddMappingMatchers(params MappingMatcher[] mappingMatchers)
        {
            MappingMatchers = MappingMatchers.Concat(mappingMatchers).Distinct().ToArray();
            return this;
        }
    }
}
