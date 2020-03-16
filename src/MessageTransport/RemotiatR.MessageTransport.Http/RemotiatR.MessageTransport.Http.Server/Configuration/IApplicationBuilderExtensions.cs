using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Server;
using System;
using System.IO;
using System.Linq;

namespace RemotiatR.MessageTransport.Http.Server
{
    public static class IApplicationBuilderExtensions
    {
        private const string _headerPrefix = "remotiatr-";

        public static IApplicationBuilder UseHttpRemotiatr(this IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions>? configure = default)
        {
            if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

            Configure<IDefaultRemotiatrMarker, IRemotiatr>(applicationBuilder, configure);

            return applicationBuilder;
        }

        public static IApplicationBuilder UseHttpRemotiatr<TMarker>(this IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions>? configure = default)
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

            applicationBuilder.MapWhen(options.MapWhenPredicate, x =>
            {
                x.Run(async httpContext =>
                {
                    var remotiatr = httpContext.RequestServices.GetRequiredService<TRemotiatr>();

                    var dataStream = new MemoryStream();
                    await httpContext.Request.Body.CopyToAsync(dataStream);
                    
                    var path = new Uri(string.Concat(
                        httpContext.Request.Scheme,
                        "://",
                        httpContext.Request.Host.ToUriComponent(),
                        httpContext.Request.PathBase.ToUriComponent(),
                        httpContext.Request.Path.ToUriComponent(),
                        httpContext.Request.QueryString.ToUriComponent()));

                    var attributes = httpContext.Request.Headers
                        .Where(x => x.Key.StartsWith(_headerPrefix))
                        .ToDictionary(x => x.Key.Substring(_headerPrefix.Length), x => x.Value.ToString());

                    var result = await remotiatr.Handle(dataStream, path, attributes);

                    foreach (var attribute in result.MessageAttributes)
                        httpContext.Response.Headers.Add(_headerPrefix + attribute.Key, attribute.Value);

                    await result.Message.CopyToAsync(httpContext.Response.Body);
                });
            });
        }
    }
}
