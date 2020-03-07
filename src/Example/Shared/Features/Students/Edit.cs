using FluentValidation;
using MediatR;
using System;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Students
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int? Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class Command : IRequest
        {
            public int Id { get; set; }
            public string LastName { get; set; }
            [DisplayName("First Name")]
            public string FirstMidName { get; set; }
            public DateTime? EnrollmentDate { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.LastName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .Length(1, 50);
                RuleFor(m => m.FirstMidName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .Length(1, 50);
                RuleFor(m => m.EnrollmentDate).NotNull();
            }
        }
    }
}
