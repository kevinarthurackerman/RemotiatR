using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.FluentValidation
{
    public static class IRemotiatrExtensions
    {
        private static ConditionalWeakTable<EditContext, ValidationMessageStore> _validationMessageStoreCache = 
            new ConditionalWeakTable<EditContext, ValidationMessageStore>();

        public static IRemotiatr<TMarker> WithValidationContext<TMarker>(this IRemotiatr<TMarker> remotiatr, EditContext editContext)
        {
            if (remotiatr == null) throw new ArgumentNullException(nameof(remotiatr));
            if (editContext == null) throw new ArgumentNullException(nameof(editContext));

            return new RemotiatrValidationWrapper<TMarker>(remotiatr, editContext);
        }

        private class RemotiatrValidationWrapper<TMarker> : IRemotiatr<TMarker>
        {
            private readonly IRemotiatr<TMarker> _remotiatr;
            private readonly EditContext _editContext;

            public RemotiatrValidationWrapper(IRemotiatr<TMarker> remotiatr, EditContext editContext)
            {
                _remotiatr = remotiatr;
                _editContext = editContext;
            }

            public async Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                if (notification == null) throw new ArgumentNullException(nameof(notification));

                try
                {
                    await _remotiatr.Send(notification, cancellationToken);

                    ClearValidation(_editContext);
                }
                catch (ValidationException exception)
                {
                    AddValidationResult(exception, _editContext);
                }
                catch(AggregateException exception) when (exception.InnerExceptions.Any(x => x is ValidationException))
                {
                    AddValidationResult(UnpackValidationException(exception), _editContext);
                }
            }

            public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            {
                if (notification == null) throw new ArgumentNullException(nameof(notification));

                try
                {
                    await _remotiatr.Send(notification, cancellationToken);

                    ClearValidation(_editContext);
                }
                catch (ValidationException exception)
                {
                    AddValidationResult(exception, _editContext);
                }
                catch (AggregateException exception) when (exception.InnerExceptions.Any(x => x is ValidationException))
                {
                    AddValidationResult(UnpackValidationException(exception), _editContext);
                }
            }

            public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                try
                {
                    var result = await _remotiatr.Send(request, cancellationToken);

                    ClearValidation(_editContext);

                    return result;
                }
                catch (ValidationException exception)
                {
                    AddValidationResult(exception, _editContext);
                }
                catch (AggregateException exception) when (exception.InnerExceptions.Any(x => x is ValidationException))
                {
                    AddValidationResult(UnpackValidationException(exception), _editContext);
                }

#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
                return default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
            }

            public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                try
                {
                    var result = await _remotiatr.Send(request, cancellationToken);

                    ClearValidation(_editContext);

                    return result;
                }
                catch (ValidationException exception)
                {
                    AddValidationResult(exception, _editContext);
                }
                catch (AggregateException exception) when (exception.InnerExceptions.Any(x => x is ValidationException))
                {
                    AddValidationResult(UnpackValidationException(exception), _editContext);
                }

                return default;
            }

            private ValidationException UnpackValidationException(AggregateException exception)
            {
                var validationExceptions = exception.Flatten()
                    .InnerExceptions
                    .Select(x => x as ValidationException)
                    .Where(x => x != null)
                    .ToArray();

                if (validationExceptions.Length == 1) return validationExceptions[0]!;

                return new ValidationException(validationExceptions.SelectMany(x => x!.Errors));
            }

            private void AddValidationResult(ValidationException validationException, EditContext editContext)
            {
                var validationMessageStore = _validationMessageStoreCache.GetValue(editContext, x => new ValidationMessageStore(x));

                validationMessageStore.Clear();

                foreach (var error in validationException.Errors)
                    validationMessageStore.Add(new FieldIdentifier(_editContext.Model, error.PropertyName), error.ErrorMessage);

                editContext.NotifyValidationStateChanged();
            }

            private void ClearValidation(EditContext editContext)
            {
                var validationMessageStore = _validationMessageStoreCache.GetValue(editContext, x => new ValidationMessageStore(x));

                validationMessageStore.Clear();

                editContext.NotifyValidationStateChanged();
            }
        }
    }
}
