using MediatR;
using RemotiatR.Example.Api.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Example.Api.Features.Todo
{
    public abstract class Ping_Info : Shared.Features.Info.Ping_Info
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IServerClock _serviceClock;

            public Handler(IServerClock serviceClock)
            {
                _serviceClock = serviceClock;
            }

            public Task<Response> Handle(Request request, CancellationToken cancellationToken) =>
                Task.FromResult(new Response { ServerTime = _serviceClock.RequestStartTime });
        }
    }
}
