using System;

namespace AutoMapper.QuickMaps
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
