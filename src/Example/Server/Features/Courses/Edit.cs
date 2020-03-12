using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Courses.Edit;

namespace ContosoUniversity.Server.Features.Courses
{
    public class Edit
    {
        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly SchoolContext _schoolContext;

            public QueryValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage("Course was not found");
            }

            private async Task<bool> BeExistingId(int? id, CancellationToken cancellationToken) =>
                id != null && await _schoolContext.Courses.AnyAsync(x => x.Id == id, cancellationToken);
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

            public async Task<Command> Handle(Query message, CancellationToken cancellationToken)
            {
                var command = await _db.Courses
                    .Where(c => c.Id == message.Id)
                    .ProjectTo<Command>(_configuration)
                    .SingleOrDefaultAsync(cancellationToken);

                command.Departments = await _db.Departments
                    .ProjectTo<Command.Department>(_configuration)
                    .ToArrayAsync(cancellationToken);

                return command;
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Course, Command>().ReverseMap();
                CreateMap<Department, Command.Department>();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly SchoolContext _schoolContext;

            public CommandValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage("Course was not found");
                RuleFor(m => m.Title).MustAsync(BeUniqueTitle)
                    .WithMessage("{PropertyName} must be unique");
                RuleFor(m => m.DepartmentId).MustAsync(BeExistingDepartmentId)
                    .WithMessage("Department was not found");
            }

            private async Task<bool> BeExistingId(int id, CancellationToken cancellationToken) =>
                await _schoolContext.Courses.AnyAsync(x => x.Id == id, cancellationToken);

            private async Task<bool> BeUniqueTitle(Command command, string title, CancellationToken cancellationToken) =>
                title == null || title == "" || !await _schoolContext.Courses.AnyAsync(x => x.Title == title && x.Id != command.Id, cancellationToken);

            private async Task<bool> BeExistingDepartmentId(int departmentId, CancellationToken cancellationToken) =>
                await _schoolContext.Departments.AnyAsync(x => x.Id == departmentId, cancellationToken);
        }

        public class CommandHandler : IRequestHandler<Command, Unit>
        {
            private readonly SchoolContext _db;
            private readonly IMapper _mapper;

            public CommandHandler(SchoolContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var course = await _db.Courses.FindAsync(request.Id);

                _mapper.Map(request, course);

                return default;
            }
        }
    }
}
