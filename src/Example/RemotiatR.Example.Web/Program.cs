using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Example.Shared;
using RemotiatR.Shared;

namespace RemotiatR.Example.Web
{
    public class Program
    {
        public static IServiceCollection Services { get; private set; }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var configuration = builder.Configuration.Build();

            var startup = new Startup(configuration);

            startup.ConfigureServices(builder.Services);

            builder.RootComponents.Add<App>("app");

            Services = builder.Services;

            var wasmHost = builder.Build();
            
            startup.Configure(wasmHost.Services);

            await wasmHost.RunAsync();
        }
    }
}
