using System;

namespace AutoMapper.QuickMaps.MappingMatchers
{
    [Flags]
    public enum MappingMatcherBehaviors
    {
        None = 0,
        MatchIdenticalName = 1,
        MatchIdenticalType = 2,
        IgnoreCase = 4
    }
}
