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
using RemotiatR.Server;
using System.Linq;
using RemotiatR.MessageTransport.Http.Server;
using RemotiatR.FluentValidation.Server;
using RemotiatR.Serializer.Json.Server;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components;

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
                
                x.AddJsonSerializer();

                x.Services.AddDbContext<SchoolContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

                x.Services.AddAutoMapper(typeof(Startup));

                x.AddFluentValidation(typeof(Program), typeof(SharedAssemblyTypeMarker));

                x.Services.AddScoped(
                    typeof(IPipelineBehavior<,>),
                    typeof(TransactionBehavior<,>));

                x.Services.AddScoped(
                    typeof(IPipelineBehavior<,>),
                    typeof(LoggingBehavior<,>));
            });

            services.AddServerSideBlazor();
            services.AddRazorPages();
            services.AddScoped(s =>
            {
                var uriHelper = s.GetRequiredService<NavigationManager>();
                return new HttpClient
                {
                    BaseAddress = new Uri(uriHelper.BaseUri)
                };
            });

            new Client.Startup(Configuration).ConfigureServices(services);

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

            app.UseHttpRemotiatr();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
