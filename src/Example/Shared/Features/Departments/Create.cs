using ContosoUniversity.Shared.Infrastructure;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Departments
{
    public class Create
    {
        public class Query : IRequest<Command> { }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.Name)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .Length(3, 50);
                RuleFor(m => m.Budget).NotNull();
                RuleFor(m => m.StartDate).NotNull();
                RuleFor(m => m.InstructorId).NotNull();
            }
        }

        public class Command : IRequest<int>
        {
            public string Name { get; set; }
            public decimal? Budget { get; set; }
            public DateTime? StartDate { get; set; }
            [DisplayName("Instructor")]
            public int? InstructorId { get; set; }
            public IEnumerable<Instructor> Administrators { get; set; }

            public class Instructor : IPerson
            {
                public int Id { get; set; }
                public string LastName { get; set; }
                [DisplayName("First Name")]
                public string FirstMidName { get; set; }
            }
        }
    }
}
