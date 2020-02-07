using FluentValidation;
using MediatR;
using System;

namespace RemotiatR.Example.Shared.Features.Todo
{
    public abstract class Create_Todos
    {
        public class Request : IRequest<Response>
        {
            public string Title { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Title)
                    .NotEmpty();
            }
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
