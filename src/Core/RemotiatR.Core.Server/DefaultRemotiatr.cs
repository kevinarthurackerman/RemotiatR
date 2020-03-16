using System;
using System.Collections.Immutable;

namespace RemotiatR.Server
{
    internal class DefaultRemotiatr : Remotiatr<IDefaultRemotiatrMarker>, IRemotiatr
    {
        internal DefaultRemotiatr(IServiceProvider serviceProvider, IServiceProvider applicationServiceProvider)
            : base(serviceProvider, applicationServiceProvider)
        {
        }
    }
}
