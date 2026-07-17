using Avalonia.Data.Converters;

namespace deavnote.app.Converters;

/// <summary>
/// Converter that converts a DateTime from UTC to local time and vice versa.
/// </summary>
internal sealed class DateTimeUtcToLocalConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTimeUtc)
        {
            return dateTimeUtc.ToLocalTime();
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTimeLocal)
        {
            return dateTimeLocal.ToUniversalTime();
        }
        return value;
    }
}
