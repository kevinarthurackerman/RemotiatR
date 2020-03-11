namespace RemotiatR.FluentValidation.Client
{
    internal class DefaultValidationErrorsAccessor : IValidationErrorsAccessor
    {
        public ValidationErrors ValidationErrors { get; } = new ValidationErrors();
    }
}
