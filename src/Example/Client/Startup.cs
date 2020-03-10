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
            services.AddRemotiatr(x =>
            {
                x.AddAssemblies(typeof(SharedAssemblyTypeMarker));
                x.SetEndpointUri(new Uri("https://localhost:44337/remotiatr"));
                x.Services.AddFluentValidation(typeof(Program), typeof(SharedAssemblyTypeMarker));
            });
        }
    }
}
