using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Server;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RemotiatR.MessageTransport.Rest.Server
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRestRemotiatr(this IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions>? configure = default)
        {
            if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

            Configure<IDefaultRemotiatrMarker, IRemotiatr>(applicationBuilder, configure);

            return applicationBuilder;
        }

        public static IApplicationBuilder UseRestRemotiatr<TMarker>(this IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions>? configure = default)
        {
            if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

            Configure<TMarker, IRemotiatr<TMarker>>(applicationBuilder, configure);

            return applicationBuilder;
        }

        private static void Configure<TMarker, TRemotiatr>(IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions>? configure = default)
            where TRemotiatr : IRemotiatr<TMarker>
        {
            if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

            var options = new UseRemotiatrOptions();
            configure?.Invoke(options);

            applicationBuilder.MapWhen(
                options.MapWhenPredicate,
                x => x.Run(async httpContext => await ProcessRequest<TMarker, TRemotiatr>(httpContext))
            );
        }

        private static async Task ProcessRequest<TMarker, TRemotiatr>(HttpContext httpContext)
            where TRemotiatr : IRemotiatr<TMarker>
        {
            var remotiatr = httpContext.RequestServices.GetRequiredService<TRemotiatr>();

            var requestContent = new MemoryStream();
            await httpContext.Request.Body.CopyToAsync(requestContent);
            requestContent.Seek(0, SeekOrigin.Begin);

            var result = await remotiatr.Handle(new Uri(httpContext.Request.GetDisplayUrl()), requestContent);

            await result.CopyToAsync(httpContext.Response.Body);
        }
    }
}
