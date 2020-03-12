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

using static ContosoUniversity.Shared.Features.Students.Edit;

namespace ContosoUniversity.Server.Features.Students
{
    public class Edit
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Student, Command>().ReverseMap();
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly SchoolContext _schoolContext;

            public QueryValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage($"Student was not found");
            }

            private async Task<bool> BeExistingId(int? id, CancellationToken cancellationToken) =>
                id.HasValue && await _schoolContext.Students.AnyAsync(x => x.Id == id, cancellationToken);
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

            public async Task<Command> Handle(Query message, CancellationToken token) => await _db.Students
                .Where(s => s.Id == message.Id)
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
                    .WithMessage($"Student was not found");
            }

            private async Task<bool> BeExistingId(int id, CancellationToken cancellationToken) =>
                await _schoolContext.Students.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly SchoolContext _db;
            private readonly IMapper _mapper;

            public CommandHandler(SchoolContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command message, CancellationToken token)
            {
                _mapper.Map(message, await _db.Students.FindAsync(message.Id));

                return default;
            }
        }
    }
}
