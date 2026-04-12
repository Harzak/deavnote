using Avalonia.Data.Converters;
using FluentIcons.Common;

namespace deavnote.app.Converters;

/// <summary>
/// Converts development task states to corresponding icon representations for UI display.
/// </summary>
internal sealed class DevTaskStateToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EDevTaskState state)
        {
            return state switch
            {
                EDevTaskState.NotStarted => Icon.Circle,
                EDevTaskState.InProgress => Icon.ArrowClockwise,
                EDevTaskState.Completed  => Icon.CheckmarkCircle,
                EDevTaskState.Merged     => Icon.Merge,
                EDevTaskState.Tested     => Icon.Beaker,
                EDevTaskState.Rejected   => Icon.DismissCircle,
                _                        => Icon.QuestionCircle,
            };
        }

        return Icon.QuestionCircle;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(DevTaskStateToIconConverter)} does not support two-way binding.");
    }
}
