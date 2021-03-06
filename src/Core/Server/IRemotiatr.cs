﻿using System;
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
        Task<Stream> Handle(Uri uri, Stream message, Action<IServiceProvider>? configure, CancellationToken cancellationToken);
    }
}
