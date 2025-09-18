using System.ComponentModel;

namespace MyProjectTemplate.Core.Helpers;

public static class EnumHelper
{
    public static string GetDescription(this Enum value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var field = value.GetType().GetField(value.ToString());

        if (field is not null)
        {
            var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attr?.Description ?? value.ToString();
        }

        return value.ToString();
    }
}