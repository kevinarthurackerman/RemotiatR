namespace RemotiatR.Client.FluentValidation
{
    internal class DefaultValidationErrorsAccessor : IValidationErrorsAccessor
    {
        public ValidationErrors ValidationErrors { get; } = new ValidationErrors();
    }
}
