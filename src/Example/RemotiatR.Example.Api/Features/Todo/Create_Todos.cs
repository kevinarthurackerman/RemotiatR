using AutoMapper;
using MediatR;
using RemotiatR.Example.Api.Services;
using RemotiatR.Example.Api.Services.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Example.Api.Features.Todo
{
    public abstract class Create_Todos : Shared.Features.Todo.Create_Todos
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
                var todo = _mapper.Map<Services.Data.TodosEntity>(request);
                todo.Created = _systemClock.RequestStartTime;
                todo.LastEdited = _systemClock.RequestStartTime;

                _dbContext.Add(todo);
                
                await _dbContext.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Response>(todo);
            }
        }
    }
}
