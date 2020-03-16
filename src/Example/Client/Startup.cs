using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContosoUniversity.Shared;
using RemotiatR.Client;
using System;
using RemotiatR.MessageTransport.Http.Client;
using RemotiatR.FluentValidation.Client;
using RemotiatR.Serializer.Json.Client;

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

                x.SetDefaultRootUri(new Uri("https://localhost:44337"));

                x.AddHttpMessageTransport();

                x.AddJsonSerializer();

                x.AddFluentValidation(typeof(Program), typeof(SharedAssemblyTypeMarker));
            });
        }
    }
}
