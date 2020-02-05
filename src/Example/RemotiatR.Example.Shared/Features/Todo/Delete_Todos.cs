using MediatR;
using System;

namespace RemotiatR.Example.Shared.Features.Todo
{
    public abstract class Delete_Todos
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }
        }

        public class Response
        {
        }
    }
}
