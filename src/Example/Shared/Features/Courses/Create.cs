using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Courses
{
    public class Create
    {
        public class Query : IRequest<Command> { }

        public class Command : IRequest<int>
        {
            public int? Number { get; set; }
            public string Title { get; set; }
            public int? Credits { get; set; }
            [DisplayName("Department")]
            public int? DepartmentId { get; set; }

            public IEnumerable<Department> Departments { get; set; }

            public class Department
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.Number).NotNull();
                RuleFor(m => m.Title)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Length(3, 50)
                    .NotNull();
                RuleFor(m => m.Credits)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .InclusiveBetween(0, 5);
                RuleFor(m => m.DepartmentId).NotNull();
            }
        }
    }
}
