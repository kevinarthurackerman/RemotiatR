using FluentValidation;
using FluentValidation.Results;
using RemotiatR.Client.MessageTransports;
using RemotiatR.Shared;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client.FluentValidation
{
    public class ValidationHttpMessageHandler : IHttpMessageHandler
    {
        private readonly IValidationErrorsAccessor _validationErrorsAccessor;
        private readonly ISerializer _serializer;

        public ValidationHttpMessageHandler(IValidationErrorsAccessor validationErrorsAccessor, ISerializer serializer)
        {
            _validationErrorsAccessor = validationErrorsAccessor ?? throw new ArgumentNullException(nameof(validationErrorsAccessor));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<HttpResponseMessage> Handle(HttpRequestMessage httpRequestMessage, HttpRequestHandlerDelegate next, CancellationToken cancellationToken = default)
        {
            if (httpRequestMessage == null) throw new ArgumentNullException(nameof(httpRequestMessage));
            if (next == null) throw new ArgumentNullException(nameof(next));

            _validationErrorsAccessor.ValidationErrors.Clear();

            var responseMessage = await next();

            if (responseMessage.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var resultStream = await responseMessage.Content.ReadAsStreamAsync();

                var errors = (ValidationError[])_serializer.Deserialize(resultStream, typeof(ValidationError[]));
                
                foreach (var error in errors)
                    _validationErrorsAccessor.ValidationErrors.Add(error);
            }

            if(_validationErrorsAccessor.ValidationErrors.Any())
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

            return responseMessage;
        }
    }
}
