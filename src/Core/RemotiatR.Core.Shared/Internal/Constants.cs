using System;

namespace RemotiatR.Shared
{
    internal static class Constants
    {
        internal static Func<Type, Uri> DefaultMessageUriLocator { get; } =
            x => new Uri("RemotiatR/" + x.FullName!.Replace('.','/').Replace('+','-'), UriKind.Relative);
    }
}
