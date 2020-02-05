using MediatR;
using System;
using System.Collections.ObjectModel;

namespace RemotiatR.Example.Shared.Features.Todo
{
    public abstract class Get_Todos
    {
        public class Request : IRequest<Response>
        {
        }

        public class Response : Collection<Response.Todo>
        {
            public class Todo
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
                public DateTime Created { get; set; }
                public DateTime LastEdited { get; set; }
            }
        }
    }
}
