using System;

namespace RemotiatR.Shared
{
    internal static class Constants
    {
        internal static Func<MessageInfo, Uri> DefaultMessageUriGenerator { get; } =
            x => new Uri(x.RequestType.FullName!.Replace('.','/').Replace('+','-'));
    }
}
