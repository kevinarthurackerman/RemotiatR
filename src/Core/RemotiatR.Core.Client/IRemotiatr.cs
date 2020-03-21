using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    public interface IRemotiatr : IRemotiatr<IDefaultRemotiatrMarker>
    {
    }

    public interface IRemotiatr<TMarker> : IMediator
    {
        Task Publish(object notification, Action<IServiceProvider>? configure, CancellationToken cancellationToken);

        Task<object> Send(object request, Action<IServiceProvider>? configure, CancellationToken cancellationToken);
    }
}
