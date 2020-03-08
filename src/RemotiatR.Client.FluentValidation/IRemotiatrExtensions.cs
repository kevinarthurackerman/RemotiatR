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

        public static IRemotiatr<TMarker> WithValidationContext<TMarker>(this IRemotiatr<TMarker> remotiatr, EditContext editContext) =>
            new RemotiatrValidationWrapper<TMarker>(remotiatr, editContext);

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
                try
                {
                    await _remotiatr.Send(notification, cancellationToken);

                    ClearValidation(_editContext);
                }
                catch (Exception exception)
                {
                    var validationException = UnpackValidationException(exception);

                    AddValidationResult(validationException, _editContext);
                }
            }

            public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            {
                try
                {
                    await _remotiatr.Send(notification, cancellationToken);

                    ClearValidation(_editContext);
                }
                catch (Exception exception)
                {
                    var validationException = UnpackValidationException(exception);

                    AddValidationResult(validationException, _editContext);
                }
            }

            public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                try
                {
                    var result = await _remotiatr.Send(request, cancellationToken);

                    ClearValidation(_editContext);

                    return result;
                }
                catch (Exception exception)
                {
                    var validationException = UnpackValidationException(exception);

                    AddValidationResult(validationException, _editContext);

                    return default;
                }
            }

            public async Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                try
                {
                    var result = await _remotiatr.Send(request, cancellationToken);

                    ClearValidation(_editContext);

                    return result;
                }
                catch (Exception exception)
                {
                    var validationException = UnpackValidationException(exception);

                    AddValidationResult(validationException, _editContext);

                    return default;
                }
            }

            private ValidationException UnpackValidationException(Exception exception)
            {
                if (exception is ValidationException validationException) return validationException;

                if (exception is AggregateException aggregateException)
                {
                    return aggregateException.Flatten()
                        .InnerExceptions
                        .Select(x => x as ValidationException)
                        .Where(x => x != null)
                        .FirstOrDefault();
                }

                return null;
            }

            private void AddValidationResult(ValidationException validationException, EditContext editContext)
            {
                var validationMessageStore = _validationMessageStoreCache.GetValue(editContext, x => new ValidationMessageStore(x));

                validationMessageStore.Clear();

                foreach (var error in validationException?.Errors)
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
