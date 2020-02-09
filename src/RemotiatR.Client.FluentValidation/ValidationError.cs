namespace RemotiatR.Client.FluentValidation
{
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
