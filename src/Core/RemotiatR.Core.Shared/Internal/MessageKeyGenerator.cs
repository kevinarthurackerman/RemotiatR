using System;

namespace RemotiatR.Shared
{
    internal static class MessageKeyGenerator
    {
        internal static Func<Type, string> Default { get; } = x => x.FullName!;
    }
}
