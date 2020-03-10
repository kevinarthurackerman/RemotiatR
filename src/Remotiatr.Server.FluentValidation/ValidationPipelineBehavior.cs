using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using RemotiatR.Shared;
using System;
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
        private readonly IMessageSerializer _serializer;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, IHttpContextAccessor httpContextAccessor, IMessageSerializer serializer)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
            _httpContext = (httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor))).HttpContext 
                ?? throw new InvalidOperationException($"{nameof(httpContextAccessor)} must provide an {nameof(HttpContext)}.");
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);
            var failures = _validators
                .Select(async v => await v.ValidateAsync(context))
                .SelectMany(result => result.Result.Errors)
                .Where(f => f != null)
                .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage, x.ErrorCode))
                .ToArray();

            if (failures.Any())
            {
                _httpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

                var responseData = await _serializer.Serialize(failures);

                await responseData.CopyToAsync(_httpContext.Response.Body);

#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
                return default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
            }

            return await next();
        }
    }
}
