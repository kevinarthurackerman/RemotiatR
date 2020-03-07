using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using ContosoUniversity.Shared;
using RemotiatR.Client.Configuration;
using System;

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
                x.SetUriBuilder(Defaults.UriBuilder);
                x.Services.AddFluentValidation(typeof(Program), typeof(SharedAssemblyTypeMarker));
            });
        }
    }
}
