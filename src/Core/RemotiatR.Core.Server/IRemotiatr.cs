using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Server
{
    public interface IRemotiatr : IRemotiatr<IDefaultRemotiatrMarker>
    {
    }

    public interface IRemotiatr<TMarker>
    {
        Task<Stream> Handle(Stream message);

        Task<Stream> Handle(Stream message, Action<IServiceProvider> configureServices);

        Task<Stream> Handle(Stream message, CancellationToken cancellationToken = default);

        Task<Stream> Handle(Stream message, Action<IServiceProvider>? configureServices, CancellationToken cancellationToken = default);
    }
}
