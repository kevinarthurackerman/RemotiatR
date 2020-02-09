using MediatR;

namespace RemotiatR.Client
{
    public interface IRemotiatr : IRemotiatr<IDefaultRemotiatrMarker>
    {
    }

    public interface IRemotiatr<TMarker> : IMediator
    {
    }
}
