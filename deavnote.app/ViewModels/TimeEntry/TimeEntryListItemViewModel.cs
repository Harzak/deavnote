namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryListItemViewModel : BaseViewModel
{
    private readonly INotificationService _notification;
    private readonly IClipboardService _clipboard;
    private readonly model.Entities.TimeEntry _model;
    public int Id { get; }

    [ObservableProperty]
    private string _taskCode;

    [ObservableProperty]
    private string _taskName;

    [ObservableProperty]
    private string _entryName;

    [ObservableProperty]
    private TimeSpan _entryDuration;

    [ObservableProperty]
    private EDevTaskState _taskState;

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

        _taskCode = timeEntry.DevTask.Code;
        _taskName = timeEntry.DevTask.Name;
        _entryName = timeEntry.Name;
        _taskState = timeEntry.DevTask.State;
        _entryDuration = timeEntry.Duration;
        this.Id = timeEntry.Id;
    }

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        await _clipboard.SetDailyTimeEntryAsync(_model).ConfigureAwait(false);
        _notification.Show($"[{_model.Name}] copied.", ENotificationType.Success);
    }
}

