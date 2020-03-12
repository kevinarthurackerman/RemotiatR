using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Server;
using System;
using System.IO;

namespace RemotiatR.MessageTransport.Http.Server.Configuration
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHttpRemotiatr(this IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions>? configure = default)
            => UseHttpRemotiatr<IDefaultRemotiatrMarker, IRemotiatr>(applicationBuilder, configure);

        public static IApplicationBuilder UseHttpRemotiatr<TMarker>(this IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions>? configure = default)
            => UseHttpRemotiatr<TMarker, IRemotiatr<TMarker>>(applicationBuilder, configure);

        private static IApplicationBuilder UseHttpRemotiatr<TMarker, TRemotiatr>(this IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions>? configure = default)
            where TRemotiatr : IRemotiatr<TMarker>
        {
            if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

            var options = new UseRemotiatrOptions();
            configure?.Invoke(options);

            applicationBuilder.MapWhen(options.MapWhenPredicate, x =>
            {
                x.Run(async httpContext =>
                {
                    var remotiatr = httpContext.RequestServices.GetRequiredService<TRemotiatr>();

                    var dataStream = new MemoryStream();
                    await httpContext.Request.Body.CopyToAsync(dataStream);

                    var result = await remotiatr.Handle(
                        dataStream, 
                        x => x.GetRequiredService<IHttpContextAccessor>().HttpContext = httpContext
                    );

                    await result.CopyToAsync(httpContext.Response.Body);
                });
            });

            return applicationBuilder;
        }
    }
}
