using Microsoft.AspNetCore.Http;
using System;

namespace RemotiatR.MessageTransport.Http.Server.Configuration
{
    public interface IUseRemotiatrOptions
    {
        IUseRemotiatrOptions SetMapWhen(Func<HttpContext,bool> mapWhenPredicate);
    }
}
