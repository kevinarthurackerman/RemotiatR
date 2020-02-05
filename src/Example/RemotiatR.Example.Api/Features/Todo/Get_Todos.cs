using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RemotiatR.Example.Api.Services.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Example.Api.Features.Todo
{
    public abstract class Get_Todos : Shared.Features.Todo.Get_Todos
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly AppDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(AppDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var todos = await _dbContext.Todos.ToArrayAsync();

                return _mapper.Map<Response>(todos);
            }
        }
    }
}
