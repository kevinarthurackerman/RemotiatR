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
            services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(Startup));

            services.AddFluentValidation(typeof(Program), typeof(SharedAssemblyTypeMarker));

            services.AddRemotiatr(x => x.AddAssemblies(typeof(Program)));

            services.AddScoped(
                typeof(IPipelineBehavior<,>),
                typeof(TransactionBehavior<,>));
            services.AddScoped(
                typeof(IPipelineBehavior<,>),
                typeof(LoggingBehavior<,>));

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

            app.UseRemotiatr(x => x.AddAssemblies(typeof(SharedAssemblyTypeMarker)));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToClientSideBlazor<Client.Program>("index.html");
            });
        }
    }
}
