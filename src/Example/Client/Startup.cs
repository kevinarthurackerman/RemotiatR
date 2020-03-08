using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContosoUniversity.Shared;
using RemotiatR.Client.Configuration;
using System;
using RemotiatR.Client.FluentValidation.Configuration;

namespace ContosoUniversity.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFluentValidation(typeof(Startup), typeof(SharedAssemblyTypeMarker));

            services.AddRemotiatr(x =>
            {
                x.AddAssemblies(typeof(SharedAssemblyTypeMarker));
                x.SetBaseUri(new Uri("https://localhost:44337"));
                x.Services.AddFluentValidation(typeof(Program), typeof(SharedAssemblyTypeMarker));
            });
        }
    }
}
