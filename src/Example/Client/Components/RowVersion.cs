using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace ContosoUniversity.Client.Components
{
    public class RowVersion : InputBase<byte[]>
    {
        protected override string FormatValueAsString(byte[] value) => 
            Encoding.Default.GetString(value);

        protected override bool TryParseValueFromString(string value, out byte[] result, out string validationErrorMessage)
        {
            result = Encoding.Default.GetBytes(value);
            validationErrorMessage = null;
            return true;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValue));
            builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddAttribute(5, "hidden", true);
            builder.CloseElement();
        }
    }
}
