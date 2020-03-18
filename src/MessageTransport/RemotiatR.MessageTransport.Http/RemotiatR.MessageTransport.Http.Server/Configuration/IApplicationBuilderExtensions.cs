using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Server;
using RemotiatR.Shared;
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

                    var attributes = GetHeaders(httpContext);

                    var path = new Uri(string.Concat(
                        httpContext.Request.Scheme,
                        "://",
                        httpContext.Request.Host.ToUriComponent(),
                        httpContext.Request.PathBase.ToUriComponent(),
                        httpContext.Request.Path.ToUriComponent(),
                        httpContext.Request.QueryString.ToUriComponent()));

                    var result = await remotiatr.Handle(dataStream, path, attributes);

                    SetHeaders(httpContext, result);

                    await result.Message.CopyToAsync(httpContext.Response.Body);
                });
            });
        }

        private static void SetHeaders(HttpContext httpContext, HandleResult result)
        {
            foreach (var attribute in result.MessageAttributes.GroupBy(x => x.Name.ToLower()))
            {
                var name = _headerPrefix + attribute.Key;
                var values = attribute.Select(x => x.Value).ToArray();
                httpContext.Response.Headers.Add(name, values);
            }
        }

        private static Attributes GetHeaders(HttpContext httpContext)
        {
            return new Attributes(httpContext.Request.Headers
                .Where(x => x.Key.StartsWith(_headerPrefix))
                .SelectMany(x => x.Value.Select(y => new Shared.Attribute(x.Key, y)))
                .ToList());
        }
    }
}
