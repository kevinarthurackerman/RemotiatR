using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.FluentValidation.Shared;
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
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private const string _metadataKey = "fv-errors";

        private readonly ConcurrentDictionary<Type, Type> _typeValidatorTypeCache = new ConcurrentDictionary<Type, Type>();

        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageMetadata _messageMetadata;

        public ValidationPipelineBehavior(IServiceProvider serviceProvider, IMessageMetadata messageMetadata)
        {
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

            if (errors.Any())
            {
                var errorValue = String.Join(',', errors.Select(x =>
                {
                    var propertyName = EscapeString(x.PropertyName);
                    var errorMessage = EscapeString(x.ErrorMessage);
                    var errorCode = EscapeString(x.ErrorCode);

                    return $"{propertyName}:{errorMessage}:{errorCode}";
                }));

                _messageMetadata.ResponseMetadata.Add(_metadataKey, errorValue);

#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
                return default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
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

        private string EscapeString(string? value) => 
            value == null ? String.Empty : value.Replace("/", "//").Replace(",", "/,").Replace(":", "/:");
    }
}
