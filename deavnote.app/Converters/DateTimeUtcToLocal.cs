using Avalonia.Data.Converters;

namespace deavnote.app.Converters;

internal sealed class DateTimeUtcToLocal : IValueConverter
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
