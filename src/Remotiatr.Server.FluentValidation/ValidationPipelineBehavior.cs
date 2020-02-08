using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using RemotiatR.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Server.FluentValidation
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly HttpContext _httpContext;
        private readonly ISerializer _serializer;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, IHttpContextAccessor httpContextAccessor, ISerializer serializer)
        {
            _validators = validators;
            _httpContext = httpContextAccessor.HttpContext;
            _serializer = serializer;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);
            var failures = _validators
                .Select(async v => await v.ValidateAsync(context))
                .SelectMany(result => result.Result.Errors)
                .Where(f => f != null)
                .Select(x => new ValidationError(x.PropertyName, x.ErrorCode, x.ErrorMessage))
                .ToArray();

            if (failures.Any())
            {
                _httpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

                var responseData = _serializer.Serialize(failures, typeof(ValidationFailure[]));

                await responseData.CopyToAsync(_httpContext.Response.Body);

                return default;
            }

            return await next();
        }
    }

    internal class ValidationError
    {
        internal ValidationError(string propertyName, string errorCode, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public string PropertyName { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
    }
}
