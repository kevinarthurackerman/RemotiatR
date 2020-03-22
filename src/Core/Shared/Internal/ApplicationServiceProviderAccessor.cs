using System;

namespace RemotiatR.Shared
{
    internal class ApplicationServiceProviderAccessor : IApplicationServiceProviderAccessor
    {
        public IServiceProvider? Value { get; set; }
    }
}
