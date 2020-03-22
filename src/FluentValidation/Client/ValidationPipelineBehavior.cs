using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using RemotiatR.FluentValidation.Shared;
using System.Text.RegularExpressions;
using RemotiatR.Shared;
using MediatR;

namespace RemotiatR.FluentValidation.Client
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private const string _metadataKey = "fv-errors";

        private readonly ConcurrentDictionary<Type, Type> _typeValidatorTypeCache = new ConcurrentDictionary<Type, Type>();

        private readonly IValidationErrorsAccessor _validationErrorsAccessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageMetadata _messageMetadata;

        public ValidationPipelineBehavior(IValidationErrorsAccessor validationErrorsAccessor, IServiceProvider serviceProvider, IMessageMetadata messageMetadata)
        {
            _validationErrorsAccessor = validationErrorsAccessor ?? throw new ArgumentNullException(nameof(validationErrorsAccessor));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _messageMetadata = messageMetadata ?? throw new ArgumentNullException(nameof(messageMetadata));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (cancellationToken == null) throw new ArgumentNullException(nameof(cancellationToken));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var context = new ValidationContext(request);

            var validators = new List<IValidator>();
            foreach (var validator in (IEnumerable)_serviceProvider.GetRequiredService(GetValidatorTypesForType(request.GetType())))
                validators.Add((IValidator)validator!);

            var errors = validators
                .Select(async v => await v.ValidateAsync(context))
                .SelectMany(result => result.Result.Errors)
                .Where(f => f != null)
                .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage, x.ErrorCode))
                .ToArray();

            if(errors.Any())
            {
                SetValidationErrors(errors);
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
                return default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
            }

            var result = await next();

            if(_messageMetadata.ResponseMetadata.TryGetValue(_metadataKey, out var errorValue))
            {
                if (errorValue == null || errorValue == String.Empty)
                    throw new InvalidOperationException($"Message metadata key '{_metadataKey}' was set, but did not have a value");

                var errorDatas = SplitWithEscape(errorValue, ',');

                if (errorDatas.Length == 0)
                    throw new InvalidOperationException($"Message metadata key '{_metadataKey}' was set, but the value '{errorValue}' could not be parsed");

                var serverErrors = new List<ValidationError>();
                foreach (var errorData in errorDatas)
                {
                    var errorArgs = SplitWithEscape(errorData, ':');

                    if (errorArgs.Length != 3)
                        throw new InvalidOperationException($"Message metadata key '{_metadataKey}' was set, but the value for error '{errorData}' could not be parsed");

                    errorDatas = errorArgs.Select(x => x.Replace("//", "/").Replace("/:", ":").Replace("/,", ",")).ToArray();

                    var propertyName = errorDatas[0];
                    var errorMessage = errorDatas[1];
                    var errorCode = errorDatas[2] == String.Empty ? null : errorDatas[2];
                    serverErrors.Add(new ValidationError(propertyName, errorMessage, errorCode));
                }

                SetValidationErrors(serverErrors);
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
                return default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
            }

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
                        })
                    .ToArray();
            }
        }

        private string[] SplitWithEscape(string value, char splitChar) =>
            Regex.Split(value, $@"(?<!($|[^\/])(\/\/)*?\/){splitChar}")
            .ToArray();
    }
}
