using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Infrastructure;
using ContosoUniversity.Server.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Students.Index;

namespace ContosoUniversity.Server.Features.Students
{
    public class Index
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Student, Model>();
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;
            private const int _pageSize = 3;

            public QueryHandler(SchoolContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Query message, CancellationToken token)
            {
                IQueryable<Student> students = _db.Students;

                if (!String.IsNullOrEmpty(message.SearchString))
                {
                    students = students.Where(s => s.LastName.Contains(message.SearchString) || s.FirstMidName.Contains(message.SearchString));
                }

                switch (message.SortOrder)
                {
                    case "name_desc":
                        students = students.OrderByDescending(s => s.LastName);
                        break;
                    case "date":
                        students = students.OrderBy(s => s.EnrollmentDate);
                        break;
                    case "date_desc":
                        students = students.OrderByDescending(s => s.EnrollmentDate);
                        break;
                    default: // Name ascending
                        students = students.OrderBy(s => s.LastName);
                        break;
                }

                int pageNumber = message.Page ?? 1;

                var model = new Result();
                model.Results = await students
                    .ProjectTo<Model>(_configuration)
                    .PaginatedListAsync(pageNumber, _pageSize);

                return model;
            }
        }
    }
}
