using FluentValidation;
using MediatR;
using System;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Students
{
    public class Create
    {
        public class Command : IRequest<int>
        {
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
