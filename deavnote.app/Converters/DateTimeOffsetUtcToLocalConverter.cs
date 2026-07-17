using Avalonia.Data.Converters;

namespace deavnote.app.Converters;

/// <summary>
/// Converter that converts a DateTimeOffset from UTC to local time and vice versa.
/// </summary>
internal sealed class DateTimeOffsetUtcToLocalConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dateTimeOffsetUtc)
        {
            return dateTimeOffsetUtc.ToLocalTime();
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dateTimeOffsetLocal)
        {
            return dateTimeOffsetLocal.ToUniversalTime();
        }
        return value;
    }
}
