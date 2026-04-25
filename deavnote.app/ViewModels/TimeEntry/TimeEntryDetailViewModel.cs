namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryDetailViewModel
    : BaseEditableViewModel<(string Name,string WorkDone, DateTimeOffset StartedAtUtc,TimeSpan Duration)>
{

    private readonly IJournal _journal;
    private readonly IViewModelFactory _factory;
    private readonly model.Entities.TimeEntry _model;

    public DateTime CreatedAtUtc => _model.CreatedAtUtc;
    public DateTime UpdatedAtUtc => _model.UpdatedAtUtc;

    [ObservableProperty]
    [Required(ErrorMessage = "Name is required.")]
    [NotifyDataErrorInfo]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial string WorkDone { get; set; }

    [ObservableProperty]
    [Required(ErrorMessage = "Start date is required.")]
    [NotifyDataErrorInfo]
    public partial DateTimeOffset StartedAtUtc { get; set; }

    [ObservableProperty]
    [Required(ErrorMessage = "Duration is required.")]
    [NotifyDataErrorInfo]
    public partial TimeSpan Duration { get; set; }

    [ObservableProperty]
    public partial DevTaskDetailViewModel? RelatedTask { get; set; }

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

        this.Name = _model.Name;
        this.WorkDone = _model.WorkDone ?? string.Empty;
        this.StartedAtUtc = new DateTimeOffset(_model.StartedAtUtc);
        this.Duration = _model.Duration;
    }

    public async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);

        this.RelatedTask = _factory.CreateDevTaskDetailViewModel(_model.DevTask, isReadonly: true);

        base.ValidateAllProperties();
        base.CommitSnapshot();
    }

    protected override async Task<OperationResult> ApplyChangesAsync(CancellationToken cancellationToken)
    {
        return await _journal.UpdateEntryAsync(new UpdateTimeEntryRequest
        {
            Id = _model.Id,
            Name = this.Name,
            WorkDone = this.WorkDone,
            StartedAtUtc = this.StartedAtUtc.DateTime,
            Duration = this.Duration,
        }, cancellationToken)
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
        return string.Equals(snapshot.Name, this.Name, StringComparison.Ordinal)
            && string.Equals(snapshot.WorkDone, this.WorkDone, StringComparison.Ordinal)
            && snapshot.StartedAtUtc == this.StartedAtUtc
            && snapshot.Duration == this.Duration;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.RelatedTask?.Dispose();
        }
        base.Dispose(disposing);
    }
}
