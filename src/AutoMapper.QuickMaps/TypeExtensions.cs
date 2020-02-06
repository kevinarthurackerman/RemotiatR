using System;
using System.Linq;

namespace AutoMapper.QuickMaps
{
    internal static class TypeExtensions
    {
        internal static string ExtendedName(this Type type) => type.FullName.Split('.').Last();
    }
}
