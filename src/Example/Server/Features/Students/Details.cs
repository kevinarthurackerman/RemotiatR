using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Students.Details;

namespace ContosoUniversity.Server.Features.Students
{
    public class Details
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Student, Model>();
                CreateMap<Enrollment, Model.Enrollment>();
            }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly SchoolContext _schoolContext;

            public QueryValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage($"Student was not found");
            }

            private async Task<bool> BeExistingId(int id, CancellationToken cancellationToken) =>
                await _schoolContext.Students.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;

            public Handler(SchoolContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public Task<Model> Handle(Query message, CancellationToken token) => _db
                    .Students
                    .Include(x => x.Enrollments)
                    .Where(s => s.Id == message.Id)
                    .ProjectTo<Model>(_configuration)
                    .SingleOrDefaultAsync(token);
        }
    }
}
