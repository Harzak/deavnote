
namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryDetailViewModel
    : BaseEditableViewModel<(
        string Name,
        string WorkDone,
        DateTimeOffset StartedAtUtc,
        TimeSpan Duration)>
{

    private readonly IJournal _journal;
    private readonly IViewModelFactory _factory;
    private readonly model.Entities.TimeEntry _model;

    public DateTime CreatedAtUtc => _model.CreatedAtUtc;
    public DateTime UpdatedAtUtc => _model.UpdatedAtUtc;

    [ObservableProperty]
    [Required(ErrorMessage = "Name is required.")]
    [NotifyDataErrorInfo]
    private string _name;

    [ObservableProperty]
    private string _workDone;

    [ObservableProperty]
    [Required(ErrorMessage = "Start date is required.")]
    [NotifyDataErrorInfo]
    private DateTimeOffset _startedAtUtc;

    [ObservableProperty]
    [Required(ErrorMessage = "Duration is required.")]
    [NotifyDataErrorInfo]
    private TimeSpan _duration;

    [ObservableProperty]
    private DevTaskDetailViewModel? _relatedTask;

    public TimeEntryDetailViewModel(
        model.Entities.TimeEntry model,
        IJournal journal,
        IViewModelFactory factory,
        INotificationService notificationService)
        : base(notificationService)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(factory);

        _model = model;
        _journal = journal;
        _factory = factory;

        _name = _model.Name;
        _workDone = _model.WorkDone ?? string.Empty;
        _startedAtUtc = _model.StartedAtUtc;
        _duration = _model.Duration;
    }

    public async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);

        _relatedTask = _factory.CreateDevTaskDetailViewModel(_model.DevTask, isReadonly: true);

        base.CommitSnapshot();
        base.ValidateAllProperties();
    }

    protected override async Task<OperationResult> ApplyChangesAsync()
    {
        return await _journal.UpdateEntryAsync(new UpdateTimeEntryRequest
        {
            Id = _model.Id,
            Name = this.Name,
            WorkDone = this.WorkDone,
            StartedAtUtc = this.StartedAtUtc.UtcDateTime,
            Duration = this.Duration
        })
        .ConfigureAwait(false);
    }

    protected override void UndoChanges(
        (string Name,
         string WorkDone,
         DateTimeOffset StartedAtUtc,
         TimeSpan Duration)
        snapshot)
    {
        this.Name = snapshot.Name;
        this.WorkDone = snapshot.WorkDone;
        this.StartedAtUtc = snapshot.StartedAtUtc;
        this.Duration = snapshot.Duration;
    }

    protected override (string Name, string WorkDone, DateTimeOffset StartedAtUtc, TimeSpan Duration) TakeSnapshot()
    {
        return (this.Name, this.WorkDone, this.StartedAtUtc, this.Duration);
    }

    protected override bool SnapshotEquals((string Name, string WorkDone, DateTimeOffset StartedAtUtc, TimeSpan Duration) snapshot)
    {
        return snapshot.Name == this.Name
            && snapshot.WorkDone == this.WorkDone
            && snapshot.StartedAtUtc == this.StartedAtUtc
            && snapshot.Duration == this.Duration;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _relatedTask?.Dispose();
        }
        base.Dispose(disposing);
    }
}
