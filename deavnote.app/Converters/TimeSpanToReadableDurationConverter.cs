using Avalonia.Data.Converters;

[assembly: InternalsVisibleTo("deavnote.app.tests")]

namespace deavnote.app.Converters;

internal sealed class TimeSpanToReadableDurationConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TimeSpan time)
        {
            return string.Empty;
        }

        if (time == TimeSpan.Zero)
        {
            return "0";
        }

        StringBuilder builder = new();

        if (time.Days > 0)
        {
            builder.Append(culture, $"{time.Days}d ");
        }
        if (time.Hours > 0)
        {
            builder.Append(culture, $"{time.Hours}h ");
        }
        if (time.Minutes > 0)
        {
            builder.Append(culture, $"{time.Minutes}m ");
        }
        if (time.Seconds > 0)
        {
            builder.Append(culture, $"{time.Seconds}s ");
        }

        return builder.ToString().TrimEnd();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(TimeSpanToReadableDurationConverter)} does not support two-way binding.");
    }
}

