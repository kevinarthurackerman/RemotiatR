using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;
using System;
using System.IO;
using System.Linq;

namespace RemotiatR.Server
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRemotiatr(this IApplicationBuilder applicationBuilder, Action<IUseRemotiatrOptions> configure = null)
        {
            var options = new UseRemotiatrOptions();
            configure?.Invoke(options);

            var requestTypes = options.AssembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsRequestType())
                .ToArray();

            var notificationTypes = options.AssembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsNotificationType())
                .ToArray();

            foreach (var notificationType in notificationTypes)
            {
                var uri = options.UriBuilder(notificationType);
                applicationBuilder.MapWhen(ctx =>
                    ctx.Request.Path == new PathString(uri.ToString())
                    && ctx.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase),
                    x => x.Run(ProcessNotification(notificationType))
                );
            }

            foreach (var requestType in requestTypes)
            {
                var uri = options.UriBuilder(requestType);
                applicationBuilder.MapWhen(ctx => 
                    ctx.Request.Path == new PathString(uri.ToString())
                    && ctx.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase), 
                    x => x.Run(ProcessRequest(requestType, requestType.GetResponseType()))
                );
            }

            return applicationBuilder;
        }

        private static RequestDelegate ProcessNotification(Type requestType) =>
            async ctx =>
            {
                var mediator = ctx.RequestServices.GetRequiredService<IMediator>();
                var serializer = ctx.RequestServices.GetRequiredService<ISerializer>();

                var dataStream = new MemoryStream();
                await ctx.Request.Body.CopyToAsync(dataStream);

                var notification = serializer.Deserialize(dataStream, requestType);

                await mediator.Send(notification);
            };

        private static RequestDelegate ProcessRequest(Type requestType, Type responseType) =>
            async ctx =>
                {
                    var mediator = ctx.RequestServices.GetRequiredService<IMediator>();
                    var serializer = ctx.RequestServices.GetRequiredService<ISerializer>();

                    var dataStream = new MemoryStream();
                    await ctx.Request.Body.CopyToAsync(dataStream);

                    var request = serializer.Deserialize(dataStream, requestType);

                    var response = await mediator.Send(request);

                    var responseData = serializer.Serialize(response, responseType);

                    await responseData.CopyToAsync(ctx.Response.Body);
                };
    }
}
