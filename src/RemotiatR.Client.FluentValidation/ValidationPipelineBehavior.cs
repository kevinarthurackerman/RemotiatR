using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.FluentValidation
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly IValidationErrorsAccessor _validationErrorsAccessor;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, IValidationErrorsAccessor validationErrorsAccessor)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
            _validationErrorsAccessor = validationErrorsAccessor ?? throw new ArgumentNullException(nameof(validationErrorsAccessor));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var context = new ValidationContext(request);

            var failures = _validators
                .Select(async v => await v.ValidateAsync(context))
                .SelectMany(result => result.Result.Errors)
                .Where(f => f != null)
                .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage, x.ErrorCode))
                .ToArray();

            foreach (var failure in failures) _validationErrorsAccessor.ValidationErrors.Add(failure);

            if (_validationErrorsAccessor.ValidationErrors.Any())
            {
                var validationFailures = _validationErrorsAccessor.ValidationErrors
                    .Select(x => new ValidationFailure(x.PropertyName, x.ErrorMessage)
                        {
                            ErrorMessage = x.ErrorMessage
                        }
                    )
                    .ToArray();

                throw new ValidationException(validationFailures);
            }

            return await next();
        }
    }
}
