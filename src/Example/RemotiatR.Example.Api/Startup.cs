using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemotiatR.Example.Api.Services;
using RemotiatR.Example.Api.Services.Data;
using AutoMapper;
using RemotiatR.Example.Shared;
using RemotiatR.Server.Configuration;
using AutoMapper.QuickMaps.Configuration;
using AutoMapper.QuickMaps.MappingMatchers;
using RemotiatR.Server.FluentValidation.Configuration;

namespace RemotiatR.Example.Api
{
    public class Startup
    {
        private const string BlazorClientPolicy = "BlazorClient";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IServerClock, ServerClock>();

            services.AddMediatR(typeof(Program));

            services.AddRemotiatr();

            services.AddAutoMapper(x => x.CreateQuickMaps(y =>
            {
                y.AddAssemblies(typeof(Program), typeof(SharedMarker));
                y.AddMappingMatchers(
                    DefaultMappingMatchers.TypeNameMatcher("{action}_{model}+Request", "{model}Entity"),
                    DefaultMappingMatchers.TypeNameMatcher("{model}Entity", "{action}_{model}+Response")
                );
            }), typeof(Program));

            services.AddFluentValidation(typeof(Program), typeof(SharedMarker));

            services.AddCors(x => x.AddPolicy(BlazorClientPolicy, x => x.WithOrigins("https://localhost:44367").AllowAnyMethod()));

            services.AddServerClock();

            services.AddEntityFrameworkInMemoryDatabase()
                .AddDbContextPool<AppDbContext>(x => x.UseInMemoryDatabase("AppDatabase"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Access-Control-Allow-Headers", "content-type");
                await next();
            });

            app.UseCors(BlazorClientPolicy);

            app.UseHttpsRedirection();

            app.UseServerClock();

            app.UseAuthorization();

            app.UseRemotiatr(x =>
            {
                x.AddAssemblies(typeof(SharedMarker));
                x.SetUriBuilder(Defaults.UriBuilder);
            });
        }
    }
}
