using System;
using System.Collections.Immutable;

namespace RemotiatR.Client
{
    internal class DefaultRemotiatr : Remotiatr<IDefaultRemotiatrMarker>, IRemotiatr
    {
        internal DefaultRemotiatr(IServiceProvider serviceProvider, IImmutableSet<Type> canHandleNotificationTypes, IImmutableSet<Type> canHandleRequestTypes)
            : base(serviceProvider, canHandleNotificationTypes, canHandleRequestTypes)
        {
        }
    }
}
