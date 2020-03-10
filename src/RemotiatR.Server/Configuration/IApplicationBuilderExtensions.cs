using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;
using RemotiatR.Shared.Internal;
using System;
using System.IO;
using System.Net;

namespace RemotiatR.Server.Configuration
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRemotiatr(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

            applicationBuilder.Map(new PathString("/remotiatr"), x =>
            {
                x.Run(async httpContext =>
                {
                    if (httpContext.Request.Method != HttpMethods.Post)
                        throw new InvalidOperationException("Must only post data to remotiatr endpoint.");

                    var mediator = httpContext.RequestServices.GetRequiredService<IMediator>();
                    var serializer = httpContext.RequestServices.GetRequiredService<IMessageSerializer>();
                    var keyMessageTypeMappings = httpContext.RequestServices.GetRequiredService<IKeyMessageTypeMappings>();
                    var httpContextAccessor = httpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();

                    httpContextAccessor.HttpContext = httpContext;

                    var dataStream = new MemoryStream();
                    await httpContext.Request.Body.CopyToAsync(dataStream);

                    var data = await serializer.Deserialize(dataStream);

                    if (!keyMessageTypeMappings.MessageTypeToKeyLookup.TryGetValue(data.GetType(), out var _))
                        throw new InvalidOperationException($"Type {data.GetType()} is not a valid message type");

                    if (data.GetType().IsNotificationType())
                    {
                        await mediator.Publish(data);
                    }
                    else if (data.GetType().IsRequestType())
                    {
                        var response = await mediator.Send(data);

                        if (IsSuccessStatusCode(httpContext.Response.StatusCode))
                        {
                            var responseData = await serializer.Serialize(response);

                            await responseData.CopyToAsync(httpContext.Response.Body);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Type {data.GetType()} was not a valid notification or request type.");
                    }
                });
            });

            return applicationBuilder;
        }

        private static bool IsSuccessStatusCode(int statusCode) => statusCode >= 200 && statusCode < 400;
    }
}
