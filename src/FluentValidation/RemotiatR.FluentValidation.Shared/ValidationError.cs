using System;

namespace RemotiatR.FluentValidation.Shared
{
    public class ValidationError
    {
        public ValidationError(string propertyName, string errorMessage, string? errorCode = null)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorCode));
            ErrorCode = errorCode;
        }

        public string PropertyName { get; }
        public string ErrorMessage { get; }
        public string? ErrorCode { get; }
    }
}
