using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Infrastructure;
using ContosoUniversity.Server.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Instructors.CreateEdit;

namespace ContosoUniversity.Server.Features.Instructors
{
    public class CreateEdit
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Instructor, Command>();
                CreateMap<Course, Command.CourseData>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(SchoolContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Command> Handle(Query message, CancellationToken token)
            {
                Command model;
                if (message.Id == null)
                {
                    model = new Command { Courses = new List<Command.CourseData>() };
                }
                else
                {
                    model = await _db.Instructors
                        .Where(i => i.Id == message.Id)
                        .ProjectTo<Command>(_configuration)
                        .SingleOrDefaultAsync(token);

                    var assignedCourseIds = await _db.CourseAssignments
                        .Where(x => x.InstructorId == message.Id)
                        .Select(x => x.CourseId)
                        .ToArrayAsync(token);

                    model.Courses = (await _db.Courses.ProjectTo<Command.CourseData>(_configuration).ToListAsync(token))
                        .Select(x =>
                        {
                            x.Assigned = assignedCourseIds.Contains(x.Id);
                            return x;
                        })
                        .ToList();
                }

                return model;
            }
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly SchoolContext _db;

            public CommandHandler(SchoolContext db) => _db = db;

            public async Task<int> Handle(Command message, CancellationToken token)
            {
                Instructor instructor;
                if (message.Id == null)
                {
                    instructor = new Instructor();
                    _db.Instructors.Add(instructor);
                }
                else
                {
                    instructor = await _db.Instructors
                        .Include(i => i.OfficeAssignment)
                        .Include(i => i.CourseAssignments)
                        .Where(i => i.Id == message.Id)
                        .SingleAsync(token);
                }

                instructor.Handle(message);

                await _db.SaveChangesAsync(token);

                return instructor.Id;
            }
        }
    }
}
