using AutoMapper;
using FluentValidation;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContosoUniversity.Server.Models;
using ContosoUniversity.Server.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using static ContosoUniversity.Shared.Features.Courses.Details;

namespace ContosoUniversity.Server.Features.Courses
{
    public class Details
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Course, Model>();
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;

            public Handler(SchoolContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public Task<Model> Handle(Query message, CancellationToken token) =>
                _db.Courses
                .Where(i => i.Id == message.Id)
                .ProjectTo<Model>(_configuration)
                .SingleOrDefaultAsync(token);
        }
    }
}
