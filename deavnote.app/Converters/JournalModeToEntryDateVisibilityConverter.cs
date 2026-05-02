using Avalonia.Data.Converters;

namespace deavnote.app.Converters;

internal sealed class JournalModeToEntryDateVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not EJournalMode journalMode)
        {
            return false;
        }

        return journalMode switch
        {
            EJournalMode.Week => true,
            EJournalMode.Month => true,
            _ => false,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(JournalModeToEntryDateVisibilityConverter)} does not support two-way binding.");
    }
}
