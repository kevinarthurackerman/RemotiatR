using System;
using System.Collections.Immutable;

namespace RemotiatR.Client
{
    internal class DefaultRemotiatr : Remotiatr<IDefaultRemotiatrMarker>, IRemotiatr
    {
        internal DefaultRemotiatr(IServiceProvider internalServiceProvider, IServiceProvider applicationService)
            : base(internalServiceProvider, applicationService)
        {
        }
    }
}
