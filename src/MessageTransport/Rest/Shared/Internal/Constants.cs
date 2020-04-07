using System;
using System.Linq;

namespace RemotiatR.MessageTransport.Rest.Shared.Internal
{
    internal static class Constants
    {
        internal static Func<Type, Uri> PathLocator = x =>
        {
            if (x == null) throw new ArgumentNullException(nameof(x));

            var path = x.FullName.Replace('.', '/').Replace('+', '-')!;

            if(x.GetProperties().Any(y => y.Name.Equals("id", StringComparison.OrdinalIgnoreCase))) path += "/{id}";

            return new Uri(path, UriKind.Relative);
        };
    }
}
