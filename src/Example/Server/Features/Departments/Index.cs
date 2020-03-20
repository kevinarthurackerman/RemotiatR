using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using ContosoUniversity.Shared.Infrastructure;
using DelegateDecompiler.EntityFrameworkCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Departments.Index;

namespace ContosoUniversity.Server.Features.Departments
{
    public class Index
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Department, Result.Model>()
                .ForMember(
                    dest => dest.AdministratorFullName,
                    opt => opt.MapFrom(src => src.Instructor.FullName())
                );
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly SchoolContext _context;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(SchoolContext context,
                IConfigurationProvider configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Query message, CancellationToken token)
            {
                var departments = await _context.Departments
                    .Include(x => x.Instructor)
                    .ProjectTo<Result.Model>(_configuration)
                    .DecompileAsync()
                    .ToListAsync(token);

                return new Result { Departments = departments };
            }
        }
    }
}
