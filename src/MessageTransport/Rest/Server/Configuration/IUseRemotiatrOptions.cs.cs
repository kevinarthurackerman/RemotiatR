using Microsoft.AspNetCore.Http;
using System;

namespace RemotiatR.MessageTransport.Rest.Server
{
    public interface IUseRemotiatrOptions
    {
        IUseRemotiatrOptions SetMapWhen(Func<HttpContext,bool> mapWhenPredicate);
    }
}
