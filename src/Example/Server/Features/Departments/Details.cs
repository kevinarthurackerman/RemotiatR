﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Server.Data;
using ContosoUniversity.Server.Models;
using ContosoUniversity.Shared.Infrastructure;
using DelegateDecompiler.EntityFrameworkCore;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static ContosoUniversity.Shared.Features.Departments.Details;

namespace ContosoUniversity.Server.Features.Departments
{
    public class Details
    {
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Department, Model>()
                .ForMember(
                    dest => dest.AdministratorFullName,
                    opt => opt.MapFrom(src => src.Instructor.FullName())
                );
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly SchoolContext _schoolContext;

            public QueryValidator(SchoolContext schoolContext)
            {
                _schoolContext = schoolContext;

                RuleFor(m => m.Id).MustAsync(BeExistingId)
                    .WithMessage("Department was not found");
            }

            private async Task<bool> BeExistingId(int id, CancellationToken cancellationToken) =>
                await _schoolContext.Departments.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly SchoolContext _context;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(SchoolContext context, IConfigurationProvider configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public Task<Model> Handle(Query message, CancellationToken token) =>
                _context.Departments
                    .Include(x => x.Instructor)
                    .Where(m => m.Id == message.Id)
                    .ProjectTo<Model>(_configuration)
                    .DecompileAsync()
                    .SingleOrDefaultAsync(token);
        }
    }
}
