﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Courses.Create;

namespace ContosoUniversity.Server.Features.Courses
{
    public partial class Create
    {
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
                var departments = await _db.Departments
                    .ProjectTo<Command.Department>(_configuration)
                    .ToArrayAsync();

                return new Command { Departments = departments };
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command, Course>();
                CreateMap<Department, Command.Department>();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly SchoolContext _schoolContext;

            public CommandValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Number).MustAsync(BeUniqueNumber)
                    .WithMessage("{PropertyName} must be unique");
                RuleFor(m => m.Title).MustAsync(BeUniqueTitle)
                    .WithMessage("{PropertyName} must be unique");
                RuleFor(m => m.DepartmentId).MustAsync(BeExistingDepartmentId)
                    .WithMessage("Department was not found");
            }

            private async Task<bool> BeUniqueNumber(int? number, CancellationToken cancellationToken) =>
                !number.HasValue || !await _schoolContext.Courses.AnyAsync(x => x.Id == number, cancellationToken);

            private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken) =>
                title == null || title == "" || !await _schoolContext.Courses.AnyAsync(x => x.Title == title, cancellationToken);

            private async Task<bool> BeExistingDepartmentId(int? departmentId, CancellationToken cancellationToken) =>
                departmentId.HasValue && await _schoolContext.Departments.AnyAsync(x => x.Id == departmentId, cancellationToken);
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly SchoolContext _db;
            private readonly IMapper _mapper;

            public CommandHandler(SchoolContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<int> Handle(Command message, CancellationToken token)
            {
                var course = _mapper.Map<Command, Course>(message);
                course.Id = message.Number.Value;

                _db.Courses.Add(course);

                await _db.SaveChangesAsync(token);

                return course.Id;
            }
        }
    }
}
