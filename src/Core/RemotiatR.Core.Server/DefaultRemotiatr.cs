using System;
using System.Collections.Immutable;

namespace RemotiatR.Server
{
    internal class DefaultRemotiatr : Remotiatr<IDefaultRemotiatrMarker>, IRemotiatr
    {
        internal DefaultRemotiatr(
            IServiceProvider serviceProvider,
            IServiceProvider applicationServiceProvider,
            IImmutableSet<Type> canHandleNotificationTypes,
            IImmutableSet<Type> canHandleRequestTypes
        )
            : base(serviceProvider, applicationServiceProvider, canHandleNotificationTypes, canHandleRequestTypes)
        {
        }
    }
}
