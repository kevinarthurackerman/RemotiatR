using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Shared;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.FluentValidation.Server
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

            if (errors.Any())
            {
                foreach (var error in errors)
                    _messageAttributes.ResponseAttributes.Add(_attributeName, $"{error.ErrorCode}:{error.PropertyName}:{error.ErrorMessage}");

                return default!;
            }

            return await next();
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
