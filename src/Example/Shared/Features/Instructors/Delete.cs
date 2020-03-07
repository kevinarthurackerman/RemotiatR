using FluentValidation;
using MediatR;
using System;

namespace ContosoUniversity.Shared.Features.Instructors
{
    public class Delete
    {
        public class Query : IRequest<Command>
        {
            public int? Id { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class Command : IRequest
        {
            public int? Id { get; set; }
            public string LastName { get; set; }
            public string FirstMidName { get; set; }
            public DateTime HireDate { get; set; }
            public string OfficeAssignmentLocation { get; set; }
        }
    }
}
