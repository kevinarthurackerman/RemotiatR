using AutoMapper;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Courses.Delete;

namespace ContosoUniversity.Server.Features.Courses
{
    public class Delete
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Course, Command>();
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly SchoolContext _schoolContext;

            public QueryValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage("{PropertyName} was not found");
            }

            private async Task<bool> BeExistingId(int? id, CancellationToken cancellationToken) =>
                id != null && await _schoolContext.Courses.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly SchoolContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(SchoolContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query message, CancellationToken token)
            {
                var course = await _db.Courses.FindAsync(new[] { message.Id }, token);
                return _mapper.Map<Command>(course);
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly SchoolContext _schoolContext;

            public CommandValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage("{PropertyName} was not found"); ;
            }

            private async Task<bool> BeExistingId(int id, CancellationToken cancellationToken) =>
                await _schoolContext.Courses.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly SchoolContext _db;

            public CommandHandler(SchoolContext db) => _db = db;

            public async Task<Unit> Handle(Command message, CancellationToken token)
            {
                var course = await _db.Courses.FindAsync(message.Id);

                _db.Courses.Remove(course);

                return default;
            }
        }
    }
}
