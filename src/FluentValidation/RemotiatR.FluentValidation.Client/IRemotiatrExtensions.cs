using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using RemotiatR.Client;
using RemotiatR.FluentValidation.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.FluentValidation.Client
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

                IValidationErrorsAccessor? errorsAccessor = null;
                await _remotiatr.Publish(notification, x => errorsAccessor = x.GetRequiredService<IValidationErrorsAccessor>()!, cancellationToken);

                AddValidationResult(errorsAccessor!.ValidationErrors, _editContext);
            }

            public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            {
                if (notification == null) throw new ArgumentNullException(nameof(notification));

                IValidationErrorsAccessor? errorsAccessor = null;
                await _remotiatr.Publish(notification, x => errorsAccessor = x.GetRequiredService<IValidationErrorsAccessor>()!, cancellationToken);

                AddValidationResult(errorsAccessor!.ValidationErrors, _editContext);
            }

            public async Task Publish(object notification, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
            {
                if (notification == null) throw new ArgumentNullException(nameof(notification));

                IValidationErrorsAccessor? errorsAccessor = null;
                await _remotiatr.Publish(notification, x => errorsAccessor = x.GetRequiredService<IValidationErrorsAccessor>()!, cancellationToken);

                AddValidationResult(errorsAccessor!.ValidationErrors, _editContext);
            }

            public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                IValidationErrorsAccessor? errorsAccessor = null;
                var result = await _remotiatr.Send<TMarker,TResponse>(request, x => errorsAccessor = x.GetRequiredService<IValidationErrorsAccessor>()!, cancellationToken);

                AddValidationResult(errorsAccessor!.ValidationErrors, _editContext);

                return result;
            }

            public async Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                IValidationErrorsAccessor? errorsAccessor = null;
                var result = await _remotiatr.Send(request, x => errorsAccessor = x.GetRequiredService<IValidationErrorsAccessor>()!, cancellationToken);

                AddValidationResult(errorsAccessor!.ValidationErrors, _editContext);

                return result;
            }

            public async Task<object> Send(object request, Action<IServiceProvider>? configure, CancellationToken cancellationToken)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                IValidationErrorsAccessor? errorsAccessor = null;
                var result = await _remotiatr.Send(request, x => errorsAccessor = x.GetRequiredService<IValidationErrorsAccessor>()!, cancellationToken);

                AddValidationResult(errorsAccessor!.ValidationErrors, _editContext);

                return result;
            }

            private void AddValidationResult(IEnumerable<ValidationError> validationException, EditContext editContext)
            {
                var validationMessageStore = _validationMessageStoreCache.GetValue(editContext, x => new ValidationMessageStore(x));

                validationMessageStore.Clear();

                foreach (var error in validationException)
                    validationMessageStore.Add(new FieldIdentifier(_editContext.Model, error.PropertyName), error.ErrorMessage);

                editContext.NotifyValidationStateChanged();
            }
        }
    }
}
