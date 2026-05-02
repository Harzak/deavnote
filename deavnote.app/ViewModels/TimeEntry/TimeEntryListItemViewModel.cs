namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryListItemViewModel : BaseViewModel
{
    private readonly INotificationService _notification;
    private readonly IClipboardService _clipboard;
    private readonly model.Entities.TimeEntry _model;

    public override string Identifier { get; }
    public int Id { get; }
    public EJournalMode ViewType { get; }

    [ObservableProperty]
    public partial string TaskCode { get; set; }

    [ObservableProperty]
    public partial string TaskName { get; set; }

    [ObservableProperty]
    public partial string EntryName { get; set; }

    [ObservableProperty]
    public partial DateOnly EntryDate { get; set; }

    [ObservableProperty]
    public partial TimeSpan EntryDuration { get; set; }

    [ObservableProperty]
    public partial EDevTaskState TaskState { get; set; }

 

    public TimeEntryListItemViewModel(
        model.Entities.TimeEntry timeEntry,
        EJournalMode journalMode,
        IClipboardService clipboard,
        INotificationService notification)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);
        ArgumentNullException.ThrowIfNull(clipboard);
        ArgumentNullException.ThrowIfNull(notification);
        _model = timeEntry;
        _clipboard = clipboard;
        _notification = notification;

        this.Identifier = Guid.NewGuid().ToString();
        this.TaskCode = timeEntry.DevTask.Code;
        this.TaskName = timeEntry.DevTask.Name;
        this.TaskState = timeEntry.DevTask.State;
        this.EntryName = timeEntry.Name;
        this.EntryDate = DateOnly.FromDateTime(timeEntry.StartedAtUtc);
        this.EntryDuration = timeEntry.Duration;
        this.ViewType = journalMode;
        this.Id = timeEntry.Id;
    }

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        await _clipboard.SetTimeEntryAsync(_model).ConfigureAwait(false);
        _notification.Show($"[{_model.Name}] copied.", ENotificationType.Success);
    }
}

