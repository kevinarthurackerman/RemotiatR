using System;

namespace RemotiatR.Shared
{
    internal static class Constants
    {
        internal static Func<Type, string> DefaultMessageKeyGenerator { get; } = x => x.FullName!;
    }
}
