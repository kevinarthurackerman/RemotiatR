using AutoMapper;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

using static ContosoUniversity.Shared.Features.Dashboard.Departments;

namespace ContosoUniversity.Server.Features.Dashboard
{
    public class Departments
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
                var departments = await _db.Departments
                    .OrderBy(x => x.Name)
                    .ProjectTo<Result.Department>(_configuration)
                    .ToArrayAsync(token);

                return new Result { Departments = departments };
            }
        }

        public class MappingProfiler : Profile
        {
            public MappingProfiler()
            {
                CreateMap<Department, Result.Department>();
            }
        }
    }
}
