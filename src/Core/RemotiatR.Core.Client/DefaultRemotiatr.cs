using System;

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
