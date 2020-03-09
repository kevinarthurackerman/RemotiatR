using System;
using System.Collections.Concurrent;

namespace RemotiatR.Shared.Internal
{
    internal static class RequestUriBuilder
    {
        private static readonly ConcurrentDictionary<Type, Uri> _uriCache = new ConcurrentDictionary<Type, Uri>();

        internal static Func<Type, Uri> DefaultUriBuilder = x => new Uri("/api/" + x.FullName.Replace('.', '/').Replace('+', '-'), UriKind.Relative);
    }
}
