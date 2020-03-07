using Microsoft.AspNetCore.Blazor.Hosting;
using System.Threading.Tasks;

namespace ContosoUniversity.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            new Startup(builder.Configuration.Build())
                .ConfigureServices(builder.Services);

            await builder.Build().RunAsync();
        }
    }
}
