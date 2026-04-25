using Avalonia.Data.Converters;
using FluentIcons.Common;

namespace deavnote.app.Converters;

/// <summary>
/// Converts a search result item type to its corresponding icon for display in the UI.
/// </summary>
internal class SearchResultItemTypeToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ESearchResultItemType state)
        {
            return state switch
            {
                ESearchResultItemType.DevTask => Icon.Note,
                ESearchResultItemType.TimeEntry => Icon.Timer,
                ESearchResultItemType.Todo => Icon.TaskList,
                _ => Icon.QuestionCircle,
            };
        }

        return Icon.QuestionCircle;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(SearchResultItemTypeToIconConverter)} does not support two-way binding.");
    }
}
