using AutoMapper;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

using static ContosoUniversity.Shared.Features.Dashboard.Instructors;

namespace ContosoUniversity.Server.Features.Dashboard
{
    public class Instructors
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
                var instructors = await _db.Instructors
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstMidName)
                    .ProjectTo<Result.Instructor>(_configuration)
                    .ToArrayAsync(token);

                return new Result { Instructors = instructors };
            }
        }

        public class MappingProfiler : Profile
        {
            public MappingProfiler()
            {
                CreateMap<Instructor, Result.Instructor>();
            }
        }
    }
}
