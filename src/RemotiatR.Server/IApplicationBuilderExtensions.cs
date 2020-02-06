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

            var requestTypesToMatch = options.AssembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsClass
                    && x.IsVisible
                    && x.GetConstructors().Any(x => !x.IsStatic && x.IsPublic)
                    && x.GetInterfaces().Any(x => x == typeof(IBaseRequest) || x == typeof(INotification))
                )
                .ToArray();

            foreach(var requestType in requestTypesToMatch)
            {
                var uri = options.UriBuilder(requestType);
                applicationBuilder.MapWhen(ctx => 
                    ctx.Request.Path == new PathString(uri.ToString())
                    && ctx.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase), 
                    x => {
                        var responseType = requestType.GetInterfaces().Any(x => x == typeof(IBaseRequest))
                            ? typeof(Unit)
                            : requestType.GetInterfaces()
                                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
                                ?.GetGenericArguments()
                                .First();

                        x.Run(async ctx =>
                        {
                            var mediator = ctx.RequestServices.GetRequiredService<IMediator>();
                            var serializer = ctx.RequestServices.GetRequiredService<ISerializer>();

                            var dataStream = new MemoryStream();
                            await ctx.Request.Body.CopyToAsync(dataStream);

                            var request = serializer.Deserialize(dataStream, requestType);

                            var response = await mediator.Send(request);

                            var responseData = serializer.Serialize(response, responseType);

                            await responseData.CopyToAsync(ctx.Response.Body);
                        });
                    }
                );
            }

            return applicationBuilder;
        }
    }
}
