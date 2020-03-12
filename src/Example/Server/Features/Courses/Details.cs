using AutoMapper;
using FluentValidation;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContosoUniversity.Server.Models;
using ContosoUniversity.Server.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using static ContosoUniversity.Shared.Features.Courses.Details;

namespace ContosoUniversity.Server.Features.Courses
{
    public class Details
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Course, Model>();
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly SchoolContext _schoolContext;

            public QueryValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage("Course was not found");
            }

            private async Task<bool> BeExistingId(int? id, CancellationToken cancellationToken) =>
                id != null && await _schoolContext.Courses.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(SchoolContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public Task<Model> Handle(Query message, CancellationToken token) =>
                _db.Courses
                .Where(i => i.Id == message.Id)
                .ProjectTo<Model>(_configuration)
                .SingleOrDefaultAsync(token);
        }
    }
}
