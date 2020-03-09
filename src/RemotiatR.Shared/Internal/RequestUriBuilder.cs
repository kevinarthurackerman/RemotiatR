using System;
using System.Collections.Concurrent;

namespace RemotiatR.Shared.Internal
{
    internal static class RequestUriBuilder
    {
        internal static Func<Type, Uri> DefaultUriBuilder = x => new Uri("/api/" + x.FullName.Replace('.', '/').Replace('+', '-'), UriKind.Relative);
    }
}
