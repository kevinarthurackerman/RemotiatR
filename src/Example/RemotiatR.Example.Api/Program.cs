using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RemotiatR.Example.Shared;
using RemotiatR.Shared;

namespace RemotiatR.Example.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureServices(x => x.Validate(typeof(Program), typeof(SharedMarker)));
                });
    }
}
