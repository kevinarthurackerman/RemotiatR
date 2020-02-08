using FluentValidation;
using MediatR;
using RemotiatR.Example.Api.Services.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Example.Api.Features.Todo
{
    public abstract class Delete_Todos : Shared.Features.Todo.Delete_Todos
    {
        public class ApiValidator : AbstractValidator<Request>
        {
            private readonly AppDbContext _dbContext;

            public ApiValidator(AppDbContext dbContext)
            {
                _dbContext = dbContext;

                RuleFor(x => x)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .MustAsync(BeExistingTodo)
                    .WithMessage("That todo no longer exists");
            }

            public async Task<bool> BeExistingTodo(Request request, CancellationToken cancellationToken)
            {
                if (request.Id == default) return false;
                var todo = await _dbContext.Todos.FindAsync(new object[] { request.Id }, cancellationToken);
                return todo != null;
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly AppDbContext _dbContext;

            public Handler(AppDbContext dbContext)
            {
                _dbContext = dbContext;
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
