using AutoMapper;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

using static ContosoUniversity.Shared.Features.Dashboard.Students;

namespace ContosoUniversity.Server.Features.Dashboard
{
    public class Students
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
                var students = await _db.Students
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstMidName)
                    .ProjectTo<Result.Student>(_configuration)
                    .ToArrayAsync(token);

                return new Result { Students = students };
            }
        }

        public class MappingProfiler : Profile
        {
            public MappingProfiler()
            {
                CreateMap<Student, Result.Student>();
            }
        }
    }
}
