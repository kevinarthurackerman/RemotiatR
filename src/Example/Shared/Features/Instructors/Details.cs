using FluentValidation;
using MediatR;
using System;

namespace ContosoUniversity.Shared.Features.Instructors
{
    public class Details
    {
        public class Query : IRequest<Model>
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

        public class Model
        {
            public int? Id { get; set; }
            public string LastName { get; set; }
            public string FirstMidName { get; set; }
            public DateTime HireDate { get; set; }
            public string OfficeAssignmentLocation { get; set; }
        }
    }
}
