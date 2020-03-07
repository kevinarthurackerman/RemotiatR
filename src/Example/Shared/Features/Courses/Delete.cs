using FluentValidation;
using MediatR;
using System.ComponentModel;

namespace ContosoUniversity.Shared.Features.Courses
{
    public class Delete
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
            [DisplayName("Number")]
            public int Id { get; set; }
            public string Title { get; set; }
            public int Credits { get; set; }
            [DisplayName("Department")]
            public string DepartmentName { get; set; }
        }
    }
}
