using FluentValidation;
using MediatR;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Courses
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
            [DisplayName("Number")]
            public int Id { get; set; }
            public string Title { get; set; }
            public int Credits { get; set; }
            [DisplayName("Department")]
            public string DepartmentName { get; set; }
        }
    }
}
