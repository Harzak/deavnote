namespace deavnote.app.ViewModels;

internal sealed partial class TimeEntryViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private TimeSpan _duration;

    [ObservableProperty]
    private EDevTaskState _state;

    public TimeEntryViewModel(TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);

        _code = timeEntry.Task.Code;
        _title = timeEntry.Name;
        _state = timeEntry.Task.State;
        _duration = timeEntry.Duration;
    }

    [RelayCommand]
    private void CopyToClipboard()
    {
        throw new NotImplementedException();
    }
}

