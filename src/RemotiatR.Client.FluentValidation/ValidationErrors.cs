using System.Collections.ObjectModel;

namespace RemotiatR.Client.FluentValidation
{
    public interface IValidationErrorsAccessor
    {
        ValidationErrors ValidationErrors { get; }
    }

    internal class ValidationErrorsAccessor : IValidationErrorsAccessor
    {
        public ValidationErrors ValidationErrors { get; } = new ValidationErrors();
    }

    public class ValidationErrors : Collection<ValidationError>
    {
    }

    public class ValidationError
    {
        public ValidationError(string propertyName, string errorCode, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public string PropertyName { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
    }
}
