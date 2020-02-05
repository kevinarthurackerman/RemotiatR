﻿using MediatR;
using System;

namespace RemotiatR.Example.Shared.Features.Todo
{
    public abstract class Edit_Todos
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public DateTime Created { get; set; }
            public DateTime LastEdited { get; set; }
        }
    }
}