using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.Shared.Features.Instructors
{
    public class CreateEdit
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

        public class Command : IRequest<int>
        {
            public int? Id { get; set; }
            public string LastName { get; set; }
            public string FirstMidName { get; set; }
            public DateTime? HireDate { get; set; }
            public string OfficeAssignmentLocation { get; set; }
            public List<CourseData> Courses { get; set; }

            public class CourseData
            {
                public int Id { get; set; }
                public string Title { get; set; }
                public bool Assigned { get; set; }
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(m => m.LastName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .Length(0, 50);
                RuleFor(m => m.FirstMidName)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .Length(0, 50);
                RuleFor(m => m.HireDate).NotNull();
            }
        }
    }
}
