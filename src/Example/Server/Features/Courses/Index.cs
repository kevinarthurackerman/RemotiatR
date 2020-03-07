using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Courses.Index;

namespace ContosoUniversity.Server.Features.Courses
{
    public class Index
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Course, Result.Course>();
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;

            public Handler(SchoolContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Query message, CancellationToken token)
            {
                var courses = await _db.Courses
                    .OrderBy(d => d.Id)
                    .ProjectTo<Result.Course>(_configuration)
                    .ToListAsync(token);

                return new Result
                {
                    Courses = courses
                };
            }
        }
    }
}
