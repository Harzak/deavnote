using Avalonia.Data;
using Avalonia.Data.Converters;

namespace deavnote.app.Converters;

/// <summary>
/// Converter that converts a Version object to its string representation and vice versa.
/// </summary>
internal sealed class VersionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Version.TryParse(value as string, out var versionStr) ? versionStr : null;
    }
}