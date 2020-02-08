using MediatR;
using System;

namespace RemotiatR.Example.Shared.Features.Info
{
    public abstract class Ping_Info
    {
        public class Request : IRequest<Response>
        {
        }

        public class Response
        {
            public DateTime ServerTime { get; set; }
        }
    }
}
