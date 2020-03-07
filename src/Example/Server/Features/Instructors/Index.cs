using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Instructors.Index;

namespace ContosoUniversity.Server.Features.Instructors
{
    public class Index
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Instructor, Model.Instructor>();
                CreateMap<CourseAssignment, Model.CourseAssignment>();
                CreateMap<Course, Model.Course>();
                CreateMap<Enrollment, Model.Enrollment>();
            }
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

            public async Task<Model> Handle(Query message, CancellationToken token)
            {
                var instructors = await _db.Instructors
                    .Include(i => i.CourseAssignments)
                    .ThenInclude(c => c.Course)
                    .OrderBy(i => i.LastName)
                    .ProjectTo<Model.Instructor>(_configuration)
                    .ToListAsync(token);

                var courses = new List<Model.Course>();
                var enrollments = new List<Model.Enrollment>();

                if (message.Id != null)
                {
                    courses = await _db.CourseAssignments
                        .Where(ci => ci.InstructorId == message.Id)
                        .Select(ci => ci.Course)
                        .ProjectTo<Model.Course>(_configuration)
                        .ToListAsync(token);
                }

                if (message.CourseId != null)
                {
                    enrollments = await _db.Enrollments
                        .Where(x => x.CourseId == message.CourseId)
                        .ProjectTo<Model.Enrollment>(_configuration)
                        .ToListAsync(token);
                }

                var viewModel = new Model
                {
                    Instructors = instructors,
                    Courses = courses,
                    Enrollments = enrollments,
                    InstructorId = message.Id,
                    CourseId = message.CourseId
                };

                return viewModel;
            }
        }
    }
}
