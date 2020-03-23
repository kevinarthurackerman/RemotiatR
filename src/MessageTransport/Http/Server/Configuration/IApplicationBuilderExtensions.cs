using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RemotiatR.MessageTransport.Http.Server
{
    public static class IApplicationBuilderExtensions
    {
        private const string _messageLengthsHeader = "message-lengths";

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
                    if(!httpContext.Request.Headers.TryGetValue(_messageLengthsHeader, out var requestMessageLengths))
                        throw new HttpRequestException($"Missing response header \"{_messageLengthsHeader}\"");

                    var requestMessageLengthInts = new List<long>();
                    foreach (var requestMessageLength in requestMessageLengths.ToString().Split(','))
                    {
                        if (!Int64.TryParse(requestMessageLength.Trim(), out var requestMessageLengthInt))
                            throw new HttpRequestException($"\"{_messageLengthsHeader}\" request header was present, but contained a non-integer value \"{requestMessageLength}\"");
                        requestMessageLengthInts.Add(requestMessageLengthInt);
                    }

                    var requestContent = new MemoryStream();
                    await httpContext.Request.Body.CopyToAsync(requestContent);
                    requestContent.Seek(0, SeekOrigin.Begin);

                    var declaredLength = requestMessageLengthInts.Aggregate((x, y) => x + y);
                    if (declaredLength != requestContent.Length)
                        throw new HttpRequestException($"\"{_messageLengthsHeader}\" request header was present, but the lengths added up to {declaredLength} while the total content length is {requestContent.Length}");

                    var remotiatr = httpContext.RequestServices.GetRequiredService<TRemotiatr>();

                    var messageTasks = new List<Task<Stream>>();
                    foreach(var requestMessageLengthInt in requestMessageLengthInts)
                    {
                        var messageBytes = new byte[requestMessageLengthInt];
                        requestContent.Read(messageBytes);
                        var message = new MemoryStream(messageBytes);

                        messageTasks.Add(remotiatr.Handle(new Uri(httpContext.Request.GetDisplayUrl()), message));
                    }

                    await Task.WhenAll(messageTasks);

                    var messageLengthsHeaderValues = messageTasks.Select(x => x.Result.Length);
                    httpContext.Response.Headers.Add(_messageLengthsHeader, String.Join(",", messageLengthsHeaderValues));

                    foreach (var messageTask in messageTasks)
                        await messageTask.Result.CopyToAsync(httpContext.Response.Body);
                });
            });
        }
    }
}
