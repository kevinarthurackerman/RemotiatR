using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContosoUniversity.Client.Components
{
    public class InputSelectNumber<T> : InputSelect<T>
    {
        private static IEnumerable<Type> _nullableNumbers = new HashSet<Type>
        {
            typeof(byte?),
            typeof(sbyte?),
            typeof(short?),
            typeof(ushort?),
            typeof(int?),
            typeof(uint?),
            typeof(long?),
            typeof(ulong?),
            typeof(float?),
            typeof(double?),
            typeof(decimal?)
        };

        protected override bool TryParseValueFromString(string value, out T result, out string validationErrorMessage)
        {
            var type = typeof(T);
            if ((value == null || value.Trim() == String.Empty) && _nullableNumbers.Contains(type))
            {
                result = default;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(byte) || type == typeof(byte?)) && byte.TryParse(value, out var byteValue))
            {
                result = (T)(object)byteValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(sbyte) || type == typeof(sbyte?)) && sbyte.TryParse(value, out var sbyteValue))
            {
                result = (T)(object)sbyteValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(short) || type == typeof(short?)) && short.TryParse(value, out var shortValue))
            {
                result = (T)(object)shortValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(ushort) || type == typeof(ushort?)) && byte.TryParse(value, out var ushortValue))
            {
                result = (T)(object)ushortValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(int) || type == typeof(int?)) && int.TryParse(value, out var intValue))
            {
                result = (T)(object)intValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(uint) || type == typeof(uint?)) && int.TryParse(value, out var uintValue))
            {
                result = (T)(object)uintValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(long) || type == typeof(long?)) && long.TryParse(value, out var longValue))
            {
                result = (T)(object)longValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(ulong) || type == typeof(ulong?)) && ulong.TryParse(value, out var ulongValue))
            {
                result = (T)(object)ulongValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(float) || type == typeof(float?)) && float.TryParse(value, out var floatValue))
            {
                result = (T)(object)floatValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(double) || type == typeof(double?)) && double.TryParse(value, out var doubleValue))
            {
                result = (T)(object)doubleValue;
                validationErrorMessage = null;
                return true;
            }
            if ((type == typeof(decimal) || type == typeof(decimal?)) && decimal.TryParse(value, out var decimalValue))
            {
                result = (T)(object)decimalValue;
                validationErrorMessage = null;
                return true;
            }
            else
            {
                result = default;
                validationErrorMessage = "The selected value is not valid.";
                return false;
            }
        }
    }
}