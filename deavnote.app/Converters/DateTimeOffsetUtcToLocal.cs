using Avalonia.Data.Converters;

namespace deavnote.app.Converters;

internal class DateTimeOffsetUtcToLocal : IValueConverter
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
