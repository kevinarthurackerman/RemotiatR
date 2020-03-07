using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity.Server.Infrastructure
{
    public class LoggingBehavior<TRequest, TResponse>
         : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehavior(ILogger<TRequest> logger)
            => _logger = logger;

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
