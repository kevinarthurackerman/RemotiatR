using Microsoft.Extensions.DependencyInjection;
using System;

namespace RemotiatR.Shared
{
    internal class ApplicationService<TService> : IApplicationService<TService>
    {
        public TService Value { get; }

        public ApplicationService(IApplicationServiceProviderAccessor applicationServiceProvider)
        {
            if (applicationServiceProvider == null) throw new ArgumentNullException(nameof(applicationServiceProvider));
            if (applicationServiceProvider.Value == null) throw new InvalidOperationException("No application service provider has been set");

            Value = applicationServiceProvider.Value.GetService<TService>()
                ?? throw new InvalidOperationException($"No service of type {typeof(TService).FullName} was found in the application service provider");
        }
    }
}
