using AutoMapper;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

using static ContosoUniversity.Shared.Features.Dashboard.Courses;

namespace ContosoUniversity.Server.Features.Dashboard
{
    public class Courses
    {
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
                    .OrderBy(x => x.Title)
                    .ProjectTo<Result.Course>(_configuration)
                    .ToArrayAsync(token);

                return new Result { Courses = courses };
            }
        }

        public class MappingProfiler : Profile
        {
            public MappingProfiler()
            {
                CreateMap<Course, Result.Course>();
            }
        }
    }
}
