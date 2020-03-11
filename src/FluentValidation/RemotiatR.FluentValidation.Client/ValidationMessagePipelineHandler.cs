using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RemotiatR.Shared;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace RemotiatR.FluentValidation.Client
{
    public class ValidationMessagePipelineHandler : IMessagePipelineHandler
    {
        private readonly ConcurrentDictionary<Type, Type> _typeValidatorTypeCache = new ConcurrentDictionary<Type, Type>();

        private readonly IValidationErrorsAccessor _validationErrorsAccessor;
        private readonly IServiceProvider _serviceProvider;

        public ValidationMessagePipelineHandler(IValidationErrorsAccessor validationErrorsAccessor, IServiceProvider serviceProvider)
        {
            _validationErrorsAccessor = validationErrorsAccessor ?? throw new ArgumentNullException(nameof(validationErrorsAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<object> Handle(object message, MessagePipelineDelegate next, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var context = new ValidationContext(message);

            var validators = new List<IValidator>();
            foreach (var validator in (IEnumerable)_serviceProvider.GetRequiredService(GetValidatorTypesForType(message.GetType())))
                validators.Add((IValidator)validator!);

            var errors = validators
                .Select(async v => await v.ValidateAsync(context))
                .SelectMany(result => result.Result.Errors)
                .Where(f => f != null)
                .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage, x.ErrorCode))
                .ToArray();

            SetValidationErrors(errors);

            var result = await next(message);

            if (result is ValidationError[] validationErrors) SetValidationErrors(validationErrors);

            return result;
        }

        private Type GetValidatorTypesForType(Type type) =>
            _typeValidatorTypeCache.GetOrAdd(type, x =>
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(x);
                var enumerableValidatorsType = typeof(IEnumerable<>).MakeGenericType(validatorType);
                return enumerableValidatorsType;
            });

        private void SetValidationErrors(IEnumerable<ValidationError> validationErrors)
        {
            foreach (var error in validationErrors) _validationErrorsAccessor.ValidationErrors.Add(error);

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
        }
    }
}
