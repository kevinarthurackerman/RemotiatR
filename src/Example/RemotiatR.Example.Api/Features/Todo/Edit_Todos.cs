using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RemotiatR.Example.Api.Services;
using RemotiatR.Example.Api.Services.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Example.Api.Features.Todo
{
    public abstract class Edit_Todos : Shared.Features.Todo.Edit_Todos
    {
        public class ApiValidator : AbstractValidator<Request>
        {
            private readonly AppDbContext _dbContext;

            public ApiValidator(AppDbContext dbContext)
            {
                _dbContext = dbContext;

                RuleFor(x => x)
                    .MustAsync(BeExistingTodo)
                    .WithMessage("That todo no longer exists");
                RuleFor(x => x.Title)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .MustAsync(BeUniqueTitle)
                    .WithMessage("{PropertyName} must be unique");
            }

            public async Task<bool> BeExistingTodo(Request request, CancellationToken cancellationToken)
            {
                if (request.Id == default) return false;
                var todo = await _dbContext.Todos.FindAsync(new object[] { request.Id }, cancellationToken);
                return todo != null;
            }

            public Task<bool> BeUniqueTitle(Request request, string title, CancellationToken cancellationToken) =>
                _dbContext.Todos.AllAsync(x => x.Title != title || x.Id == request.Id, cancellationToken);
        }

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
