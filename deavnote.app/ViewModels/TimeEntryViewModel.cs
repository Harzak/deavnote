using deavnote.app.ViewModels.Base;

namespace deavnote.app.ViewModels;

internal sealed partial class TimeEntryViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private TimeSpan _duration;

    public TimeEntryViewModel(TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);

        _code = timeEntry.Code;
        _title = timeEntry.Task.Name;
        _duration = timeEntry.Duration;
    }

}

