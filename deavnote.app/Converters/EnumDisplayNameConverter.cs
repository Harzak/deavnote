using Avalonia.Data.Converters;
using System.Reflection;

[assembly: InternalsVisibleTo("deavnote.app.tests")]

namespace deavnote.app.Converters;

/// <summary>
/// Converter that extracts the Display attribute name from an enum value.
/// Falls back to the enum name if no Display attribute is found.
/// </summary>
internal sealed class EnumDisplayNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return "N/A";
        }

        if (value is Enum enumValue)
        {
            FieldInfo? field = enumValue.GetType().GetField(enumValue.ToString());
            if (field != null)
            {
                DisplayAttribute? displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name))
                {
                    return displayAttribute.Name;
                }
            }
            return enumValue.ToString();
        }

        return value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(EnumDisplayNameConverter)} does not support two-way binding.");
    }
}