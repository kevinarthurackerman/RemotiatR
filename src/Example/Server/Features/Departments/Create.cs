using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Departments.Create;

namespace ContosoUniversity.Server.Features.Departments
{
    public class Create
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
                var administrators = await _db.Instructors
                    .ProjectTo<Command.Instructor>(_configuration)
                    .ToArrayAsync(token);

                return new Command { Administrators = administrators };
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly SchoolContext _schoolContext;

            public CommandValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Name).MustAsync(BeUniqueName)
                    .WithMessage("{PropertyName} must be unique");
                RuleFor(m => m.InstructorId).MustAsync(BeExistingInstructorId)
                    .WithMessage("Deparment was not found");
            }

            private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken) =>
                !await _schoolContext.Departments.AnyAsync(x => x.Name == name, cancellationToken);

            private async Task<bool> BeExistingInstructorId(int? instructorId, CancellationToken cancellationToken) =>
                instructorId.HasValue && await _schoolContext.Instructors.AnyAsync(x => x.Id == instructorId, cancellationToken);
        }

        public class MappingProfiler : Profile
        {
            public MappingProfiler()
            {
                CreateMap<Command, Department>();
                CreateMap<Instructor, Command.Instructor>();
            }
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly SchoolContext _context;
            private readonly IMapper _mapper;

            public CommandHandler(SchoolContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<int> Handle(Command message, CancellationToken token)
            {
                var department = _mapper.Map<Command, Department>(message);

                _context.Departments.Add(department);

                await _context.SaveChangesAsync(token);

                return department.Id;
            }
        }
    }
}
