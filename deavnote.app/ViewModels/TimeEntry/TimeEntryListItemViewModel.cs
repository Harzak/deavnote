namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryListItemViewModel : BaseViewModel
{
    public int Id { get; }

    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private TimeSpan _duration;

    [ObservableProperty]
    private EDevTaskState _state;

    public TimeEntryListItemViewModel(model.Entities.TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);

        _code = timeEntry.Task.Code;
        _title = timeEntry.Name;
        _state = timeEntry.Task.State;
        _duration = timeEntry.Duration;
        this.Id = timeEntry.Id;
    }

    [RelayCommand]
    private void CopyToClipboard()
    {
        throw new NotImplementedException();
    }
}

