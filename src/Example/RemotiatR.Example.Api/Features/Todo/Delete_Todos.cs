using AutoMapper;
using MediatR;
using RemotiatR.Example.Api.Services.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Example.Api.Features.Todo
{
    public abstract class Delete_Todos : Shared.Features.Todo.Delete_Todos
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
                var todo = await _dbContext.Todos.FindAsync(request.Id);

                _dbContext.Remove(todo);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return new Response();
            }
        }
    }
}
