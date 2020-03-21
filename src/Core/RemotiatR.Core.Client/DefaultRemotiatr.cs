using System;
using System.Collections.Immutable;

namespace RemotiatR.Client
{
    internal class DefaultRemotiatr : Remotiatr<IDefaultRemotiatrMarker>, IRemotiatr
    {
        internal DefaultRemotiatr(
            IServiceProvider internalServiceProvider,
            IServiceProvider applicationService,
            IImmutableSet<Type> canHandleNotificationTypes,
            IImmutableSet<Type> canHandleRequestTypes
        )
            : base(internalServiceProvider, applicationService, canHandleNotificationTypes, canHandleRequestTypes)
        {
        }
    }
}
