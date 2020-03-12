using Microsoft.AspNetCore.Http;
using System;

namespace RemotiatR.MessageTransport.Http.Server
{
    public interface IUseRemotiatrOptions
    {
        IUseRemotiatrOptions SetMapWhen(Func<HttpContext,bool> mapWhenPredicate);
    }
}
