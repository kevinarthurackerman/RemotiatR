using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.QuickMaps;
using RemotiatR.Example.Shared;
using RemotiatR.Client;
using System.Linq;
using System;

namespace RemotiatR.Example.Web
{
    public class Program
    {
        public static IServiceCollection Services { get; private set; }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("app");

            builder.Services.AddAutoMapper(x => x.CreateQuickMaps(y =>
            {
                y.AddAssemblies(typeof(Program), typeof(SharedMarker));
                y.AddMappingMatchers(
                    DefaultMappingMatchers.TypeNameMatcher("{action}_{model}+Response", "{view}+{model}ViewModel"),
                    DefaultMappingMatchers.TypeNameMatcher("{view}+{model}ViewModel", "{action}_{model}+Request")
                );
            }), typeof(Program));

            builder.Services.AddRemotiatr(x => x.AddDefaultServer(x =>
            {
                x.AddAssemblies(typeof(SharedMarker));
                x.SetUriBuilder(x =>
                {
                    var segments = x.FullName.Split('.').Last().Split('+').First().Split('_');
                    var uri = $"https://localhost:44339/api/{segments[1]}/{segments[0]}";
                    return new Uri(uri);
                });
            }));

            Services = builder.Services;

            var wasmHost = builder.Build();

            var httpClient = wasmHost.Services.GetRequiredService<HttpClient>();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            await wasmHost.RunAsync();
        }
    }
}
