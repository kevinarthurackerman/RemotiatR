using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Server
{
    public static class IRemotiatrExtensions
    {
        public static Task<Stream> Handle<TMarker>(this IRemotiatr<TMarker> remotiatr, Stream message) =>
            remotiatr.Handle(message, default, default);

        public static Task<Stream> Handle<TMarker>(this IRemotiatr<TMarker> remotiatr, Stream message, Action<IServiceProvider> configure) =>
            remotiatr.Handle(message, configure, default);

        public static Task<Stream> Handle<TMarker>(this IRemotiatr<TMarker> remotiatr, Stream message, CancellationToken cancellationToken) =>
            remotiatr.Handle(message, default, cancellationToken);
    }
}
