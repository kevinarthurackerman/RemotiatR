using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemotiatR.Client.FluentValidation
{
    public interface IValidationErrorsAccessor
    {
        ValidationErrors ValidationErrors { get; set; }
    }

    internal class ValidationErrorsAccessor : IValidationErrorsAccessor
    {
        public ValidationErrors ValidationErrors { get; set; }
    }

    public class ValidationErrors : ReadOnlyCollection<ValidationError>
    {
        internal ValidationErrors(IList<ValidationError> list) : base(list)
        {
        }
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
