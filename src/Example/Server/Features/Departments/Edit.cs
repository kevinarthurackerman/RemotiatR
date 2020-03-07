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

using static ContosoUniversity.Shared.Features.Departments.Edit;

namespace ContosoUniversity.Server.Features.Departments
{
    public class Edit
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Department, Command>().ReverseMap();
                CreateMap<Instructor, Command.Instructor>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(SchoolContext db,
                IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Command> Handle(Query message, CancellationToken token)
            {
                var command = await _db
                    .Departments
                    .Where(d => d.Id == message.Id)
                    .ProjectTo<Command>(_configuration)
                    .SingleOrDefaultAsync(token);

                command.Administrators = await _db.Instructors
                    .ProjectTo<Command.Instructor>(_configuration)
                    .ToArrayAsync(token);

                return command;
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
                    var dept = await _db.Departments.FindAsync(message.Id);

                    _mapper.Map(message, dept);

                    return default;
                }
            }
        }
    }
}
