using AutoMapper;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Students.Create;

namespace ContosoUniversity.Server.Features.Students
{
    public class Create
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Command, Student>(MemberList.Source);
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly SchoolContext _db;
            private readonly IMapper _mapper;

            public Handler(SchoolContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<int> Handle(Command message, CancellationToken token)
            {
                var student = _mapper.Map<Command, Student>(message);

                _db.Students.Add(student);

                await _db.SaveChangesAsync(token);

                return student.Id;
            }
        }
    }
}
