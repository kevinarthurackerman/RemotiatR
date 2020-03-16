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
using MediatR;

namespace RemotiatR.FluentValidation.Client
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest,TResponse>
    {
        private const string _attributeName = "fluent-validation-error";

        private readonly ConcurrentDictionary<Type, Type> _typeValidatorTypeCache = new ConcurrentDictionary<Type, Type>();

        private readonly IServiceProvider _serviceProvider;
        private readonly MessageAttributes _messageAttributes;

        public ValidationPipelineBehavior(IServiceProvider serviceProvider, MessageAttributes messageAttributes)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _messageAttributes = messageAttributes ?? throw new ArgumentNullException(nameof(messageAttributes));
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
                .ToArray();

            if (errors.Any()) throw new ValidationException(errors);

            var result = await next();

            errors = _messageAttributes.ResponseAttributes.Get(_attributeName)
                .Select(x =>
                {
                    var parts = x.Value.Split(':');
                    if (parts.Length != 3) throw new InvalidOperationException($"Tried to parse value '{x.Value}' of {_attributeName} attribute, but it was not valid. Value must be in the format '{{errorCode}}:{{propertyName}}:{{errorMessage}}'");
                    var errorCode = parts[0];
                    var propertyName = parts[1];
                    var errorMessage = parts[2];

                    return new ValidationFailure(propertyName, errorMessage)
                    {
                        ErrorCode = errorCode
                    };
                })
                .ToArray();

            if (errors.Any()) throw new ValidationException(errors);

            return result;
        }

        private Type GetValidatorTypesForType(Type type) =>
            _typeValidatorTypeCache.GetOrAdd(type, x =>
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(x);
                var enumerableValidatorsType = typeof(IEnumerable<>).MakeGenericType(validatorType);
                return enumerableValidatorsType;
            });
    }
}
