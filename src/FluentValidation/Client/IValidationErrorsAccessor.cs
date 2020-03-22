using RemotiatR.FluentValidation.Shared;

namespace RemotiatR.FluentValidation.Client
{
    public interface IValidationErrorsAccessor
    {
        ValidationErrors ValidationErrors { get; }
    }
}
