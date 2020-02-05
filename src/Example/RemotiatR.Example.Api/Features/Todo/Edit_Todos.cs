using AutoMapper;
using MediatR;
using RemotiatR.Example.Api.Services;
using RemotiatR.Example.Api.Services.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Example.Api.Features.Todo
{
    public abstract class Edit_Todos : Shared.Features.Todo.Edit_Todos
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly AppDbContext _dbContext;
            private readonly IServerClock _systemClock;
            private readonly IMapper _mapper;

            public Handler(AppDbContext dbContext, IServerClock systemClock, IMapper mapper)
            {
                _dbContext = dbContext;
                _systemClock = systemClock;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var todo = await _dbContext.Todos.FindAsync(request.Id);
                todo.LastEdited = _systemClock.RequestStartTime;

                _mapper.Map(request, todo);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Response>(todo);
            }
        }
    }
}
