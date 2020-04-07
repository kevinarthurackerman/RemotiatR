using Microsoft.AspNetCore.Http;
using System;

namespace RemotiatR.MessageTransport.Rest.Server
{
    internal class UseRemotiatrOptions : IUseRemotiatrOptions
    {
        internal Func<HttpContext, bool> MapWhenPredicate { get; private set; } = httpContext =>
            httpContext.Request.Path.Equals(httpContext.Request.PathBase + "/remotiatr", StringComparison.OrdinalIgnoreCase)
                && httpContext.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase);

        public IUseRemotiatrOptions SetMapWhen(Func<HttpContext, bool> mapWhenPredicate)
        {
            MapWhenPredicate = mapWhenPredicate ?? throw new ArgumentNullException(nameof(mapWhenPredicate));

            return this;
        }
    }
}
