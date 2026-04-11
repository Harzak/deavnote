namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryListItemViewModel : BaseViewModel
{
    private readonly INotificationService _notification;
    private readonly IClipboardService _clipboard;
    private readonly model.Entities.TimeEntry _model;
    public int Id { get; }

    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private TimeSpan _duration;

    [ObservableProperty]
    private EDevTaskState _state;

    public TimeEntryListItemViewModel(
        model.Entities.TimeEntry timeEntry, 
        IClipboardService clipboard,
        INotificationService notification)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);
        ArgumentNullException.ThrowIfNull(clipboard);
        ArgumentNullException.ThrowIfNull(notification);
        _model = timeEntry;
        _clipboard = clipboard;
        _notification = notification;

        _code = timeEntry.Task.Code;
        _title = timeEntry.Name;
        _state = timeEntry.Task.State;
        _duration = timeEntry.Duration;
        this.Id = timeEntry.Id;
    }

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        await _clipboard.SetDailyTimeEntryAsync(_model).ConfigureAwait(false);
        _notification.Show($"[{_model.Name}] copied.", ENotificationType.Success);
    }
}

