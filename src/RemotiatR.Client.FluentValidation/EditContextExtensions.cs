using Microsoft.AspNetCore.Components.Forms;
using System.Linq;

namespace RemotiatR.Client.FluentValidation
{
    public static class EditContextExtensions
    {
        public static bool IsValid(this EditContext editContext) => !editContext.GetValidationMessages().Any();
    }
}
