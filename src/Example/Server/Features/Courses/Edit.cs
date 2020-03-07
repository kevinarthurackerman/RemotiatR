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
