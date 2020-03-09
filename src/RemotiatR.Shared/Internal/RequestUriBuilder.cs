using System;
using System.Collections.Concurrent;

namespace RemotiatR.Shared.Internal
{
    internal static class RequestUriBuilder
    {
        private static readonly ConcurrentDictionary<Type, Uri> _uriCache = new ConcurrentDictionary<Type, Uri>();

        internal static Func<Type, Uri> DefaultUriBuilder = x => 
            _uriCache.GetOrAdd(x, x =>
            {
                if (x == null) throw new ArgumentNullException(nameof(x));

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return new Uri("/api/" + x.FullName.Replace('.', '/').Replace('+', '-'), UriKind.Relative);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
    }
}
