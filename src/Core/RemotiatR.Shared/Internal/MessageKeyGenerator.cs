using System;

namespace RemotiatR.Shared.Internal
{
    internal static class MessageKeyGenerator
    {
        internal static Func<Type, string> Default { get; } = x => x.FullName!;
    }
}
