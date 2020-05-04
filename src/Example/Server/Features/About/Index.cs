using ContosoUniversity.Server.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.About.Index;

namespace ContosoUniversity.Server.Features.About
{
    public class Index
    {
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly SchoolContext _db;

            public Handler(SchoolContext db) => _db = db;

            public async Task<Result> Handle(Query message, CancellationToken token)
            {
                var groups = await _db.Students
                    .GroupBy(x => x.EnrollmentDate)
                    .Select(x => new Result.EnrollmentDateGroup
                    {
                        EnrollmentDate = x.Key,
                        StudentCount = x.Count()
                    })
                    .ToArrayAsync(token);

                return new Result { EnrollmentDateGroups = groups };
            }
        }
    }
}
