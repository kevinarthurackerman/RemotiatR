using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using RemotiatR.Shared;

namespace ContosoUniversity.Server.Infrastructure
{
    public class LoggingBehavior<TRequest, TResponse>
         : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehavior(IApplicationService<ILogger<TRequest>> logger)
            => _logger = logger.Value;

        public async Task<TResponse> Handle(
            TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            using (_logger.BeginScope(request))
            {
                _logger.LogInformation("Calling handler...");
                var response = await next();
                _logger.LogInformation("Called handler with result {0}", response);
                return response;
            }
        }
    }
}
