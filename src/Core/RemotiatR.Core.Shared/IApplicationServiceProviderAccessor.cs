using System;

namespace RemotiatR.Shared
{
    public interface IApplicationServiceProviderAccessor
    {
        IServiceProvider? Value { get; set; }
    }
}
