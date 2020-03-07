using FluentValidation;
using MediatR;
using System;
using System.ComponentModel;

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
            [DisplayName("First Name")]
            public string FirstMidName { get; set; }
            public DateTime HireDate { get; set; }
            [DisplayName("Office Location")]
            public string OfficeAssignmentLocation { get; set; }
        }
    }
}
