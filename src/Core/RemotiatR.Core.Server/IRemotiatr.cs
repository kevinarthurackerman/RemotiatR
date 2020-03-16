using System;
using System.Collections.Generic;
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
        Task<HandleResult> Handle(Stream message, Uri messagePath, IDictionary<string, string> messageAttributes, CancellationToken cancellationToken = default);
    }
}
