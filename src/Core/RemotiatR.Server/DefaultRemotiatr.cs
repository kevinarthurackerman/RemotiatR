using System;
using System.Collections.Immutable;

namespace RemotiatR.Server
{
    internal class DefaultRemotiatr : Remotiatr<IDefaultRemotiatrMarker>, IRemotiatr
    {
        internal DefaultRemotiatr(IServiceProvider serviceProvider, IImmutableSet<Type> canHandleNotificationTypes, IImmutableSet<Type> canHandleRequestTypes)
            : base(serviceProvider, canHandleNotificationTypes, canHandleRequestTypes)
        {
        }
    }
}
