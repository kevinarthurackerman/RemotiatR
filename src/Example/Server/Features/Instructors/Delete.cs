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

using static ContosoUniversity.Shared.Features.Instructors.Delete;

namespace ContosoUniversity.Server.Features.Instructors
{
    public class Delete
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Instructor, Command>();
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly SchoolContext _schoolContext;

            public QueryValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage("Instructor was not found");
            }

            private async Task<bool> BeExistingId(int? id, CancellationToken cancellationToken) =>
                id.HasValue && await _schoolContext.Instructors.AnyAsync(x => x.Id == id, cancellationToken);
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

            public Task<Command> Handle(Query message, CancellationToken token) => _db
                .Instructors
                .Where(i => i.Id == message.Id)
                .ProjectTo<Command>(_configuration)
                .SingleOrDefaultAsync(token);
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly SchoolContext _schoolContext;

            public CommandValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage("Instructor was not found");
            }

            private async Task<bool> BeExistingId(int? id, CancellationToken cancellationToken) =>
                id.HasValue && await _schoolContext.Instructors.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly SchoolContext _db;

            public CommandHandler(SchoolContext db) => _db = db;

            public async Task<Unit> Handle(Command message, CancellationToken token)
            {
                var instructor = await _db.Instructors.FindAsync(message.Id);

                _db.Instructors.Remove(instructor);

                return Unit.Value;
            }
        }
    }
}
