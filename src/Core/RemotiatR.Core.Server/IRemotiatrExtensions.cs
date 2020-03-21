using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Server
{
    public static class IRemotiatrExtensions
    {
        public static Task<Stream> Handle<TMarker>(this IRemotiatr<TMarker> remotiatr, Uri uri, Stream message) =>
            remotiatr.Handle(uri, message, default, default);

        public static Task<Stream> Handle<TMarker>(this IRemotiatr<TMarker> remotiatr, Uri uri, Stream message, Action<IServiceProvider> configure) =>
            remotiatr.Handle(uri, message, configure, default);

        public static Task<Stream> Handle<TMarker>(this IRemotiatr<TMarker> remotiatr, Uri uri, Stream message, CancellationToken cancellationToken) =>
            remotiatr.Handle(uri, message, default, cancellationToken);
    }
}
