using Microsoft.AspNetCore.Http;
using System;

namespace RemotiatR.Server.Configuration
{
    public interface IUseRemotiatrOptions
    {
        IUseRemotiatrOptions SetMapWhen(Func<HttpContext,bool> mapWhenPredicate);
    }
}
