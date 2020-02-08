using RemotiatR.Shared;
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
            _validationErrorsAccessor = validationErrorsAccessor;
            _serializer = serializer;
        }

        public async Task<HttpResponseMessage> Handle(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken, HttpRequestHandlerDelegate next)
        {
            var responseMessage = await next();

            if (responseMessage.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var resultStream = await responseMessage.Content.ReadAsStreamAsync();

                var errors = (ValidationError[])_serializer.Deserialize(resultStream, typeof(ValidationError[]));

                _validationErrorsAccessor.ValidationErrors = new ValidationErrors(errors.ToList());
            }

            return responseMessage;
        }
    }
}
