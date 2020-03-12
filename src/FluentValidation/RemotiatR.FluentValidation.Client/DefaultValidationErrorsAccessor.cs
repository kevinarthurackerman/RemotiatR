using RemotiatR.FluentValidation.Shared;

namespace RemotiatR.FluentValidation.Client
{
    internal class DefaultValidationErrorsAccessor : IValidationErrorsAccessor
    {
        public ValidationErrors ValidationErrors { get; } = new ValidationErrors();
    }
}
