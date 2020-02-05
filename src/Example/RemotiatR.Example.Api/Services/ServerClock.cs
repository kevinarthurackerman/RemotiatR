using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace RemotiatR.Example.Api.Services
{
    public class ServerClock : IServerClock
    {
        public DateTime Now => DateTime.UtcNow;
        public DateTime RequestStartTime { get; } = DateTime.UtcNow;
    }

    public interface IServerClock
    {
        DateTime Now { get; }
        DateTime RequestStartTime { get; }
    }

    public static class ServerClockExtensions
    {
        public static IServiceCollection AddServerClock(this IServiceCollection serviceCollection) => serviceCollection.AddScoped<IServerClock, ServerClock>();

        public static IApplicationBuilder UseServerClock(this IApplicationBuilder applicationBuilder) =>
            applicationBuilder.Use(async (context, next) =>
            {
                context.RequestServices.GetRequiredService<IServerClock>();
                await next();
            });
    }
}
