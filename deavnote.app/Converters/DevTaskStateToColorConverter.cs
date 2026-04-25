using Avalonia.Data.Converters;
using Avalonia.Media;

namespace deavnote.app.Converters;

/// <summary>
/// Converts development task states to corresponding brush colors for UI representation.
/// </summary>
internal sealed class DevTaskStateToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EDevTaskState state)
        {
            return state switch
            {
                EDevTaskState.NotStarted => Brushes.WhiteSmoke,
                EDevTaskState.InProgress => Brushes.Orange,
                EDevTaskState.Completed => Brushes.LightGreen,
                EDevTaskState.Merged => Brushes.LightSkyBlue,
                EDevTaskState.Tested => Brushes.DeepPink,
                EDevTaskState.Rejected => Brushes.OrangeRed,
                _ => Brushes.Gray,
            };
        }

        return Brushes.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(DevTaskStateToColorConverter)} does not support two-way binding.");
    }
}
