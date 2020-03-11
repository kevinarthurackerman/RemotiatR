using AutoMapper;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Infrastructure;
using ContosoUniversity.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemotiatR.Server.Configuration;
using RemotiatR.Server.FluentValidation.Configuration;
using System.Linq;

namespace ContosoUniversity.Server
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
                x.AddAssemblies(typeof(Program), typeof(SharedAssemblyTypeMarker));

                foreach (var service in services) x.Services.Add(service);

                x.Services.AddDbContext<SchoolContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

                x.Services.AddAutoMapper(typeof(Startup));

                x.Services.AddFluentValidation(typeof(Program), typeof(SharedAssemblyTypeMarker));

                x.Services.AddScoped(
                    typeof(IPipelineBehavior<,>),
                    typeof(TransactionBehavior<,>));

                x.Services.AddScoped(
                    typeof(IPipelineBehavior<,>),
                    typeof(LoggingBehavior<,>));
            });

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseStaticFiles();
            app.UseClientSideBlazorFiles<Client.Program>();

            app.UseRouting();

            app.UseRemotiatr();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToClientSideBlazor<Client.Program>("index.html");
            });
        }
    }
}
