namespace RemotiatR.Server.FluentValidation
{
    internal class ValidationError
    {
        internal ValidationError(string propertyName, string errorMessage, string errorCode)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public string PropertyName { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
    }
}
