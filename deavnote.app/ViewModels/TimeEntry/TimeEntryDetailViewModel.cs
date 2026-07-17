namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryDetailViewModel
    : BaseEditableViewModel<TimeEntrySnapshot>
{
    private readonly IJournal _journal;
    private readonly IDevTaskRepository _taskRepository;
    private readonly model.Entities.TimeEntry _model;

    public override string EditedElementIdentifier { get; }

    #region Task properties
    public DateTime TaskCreatedAt { get; set; }
    public DateTime TaskUpdatedAt { get; set; }
    public string TaskCode { get; set; }

    [ObservableProperty]
    [Required(ErrorMessage = "Name is required.")]
    [NotifyDataErrorInfo]
    public partial string TaskName { get; set; }

    [ObservableProperty]
    public partial string TaskDescription { get; set; }

    [ObservableProperty]
    public partial EDevTaskState TaskState { get; set; }

    [ObservableProperty]
    public partial Version? TaskRelease { get; set; }
    #endregion

    #region Time entry properties
    [ObservableProperty]
    [Required(ErrorMessage = "Name is required.")]
    [NotifyDataErrorInfo]
    public partial string EntryName { get; set; }

    [ObservableProperty]
    [Required(ErrorMessage = "Start date is required.")]
    [NotifyDataErrorInfo]
    public partial DateTimeOffset EntryStartedAt { get; set; }

    [ObservableProperty]
    [Required(ErrorMessage = "Duration is required.")]
    [NotifyDataErrorInfo]
    public partial TimeSpan EntryDuration { get; set; }
    #endregion

    public TimeEntryDetailViewModel(
        model.Entities.TimeEntry model,
        IJournal journal,
        IDevTaskRepository taskRepository,
        INotificationService notificationService)
        : base(notificationService)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(taskRepository);

        _model = model;
        _journal = journal;
        _taskRepository = taskRepository;

        this.EditedElementIdentifier = _model.Id.ToString(CultureInfo.InvariantCulture);

        this.TaskName = string.Empty;
        this.TaskDescription = string.Empty;
        this.TaskCode = string.Empty;
        this.TaskState = EDevTaskState.Unknown;

        this.EntryName = _model.Name;
        DateTime startedAtUtc = DateTime.SpecifyKind(_model.StartedAtUtc, DateTimeKind.Utc);
        this.EntryStartedAt = new DateTimeOffset(startedAtUtc);
        this.EntryDuration = _model.Duration;
    }

    public async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);

        model.Entities.DevTask? relatedTask = await _taskRepository.GetTaskAsync(_model.TaskId).ConfigureAwait(false);
        if (relatedTask != null)
        {
            this.TaskName = relatedTask.Name;
            this.TaskCode = relatedTask.Code;
            this.TaskState = relatedTask.State;
            this.TaskRelease = relatedTask.Release;
            this.TaskCreatedAt = relatedTask.CreatedAtUtc;
            this.TaskUpdatedAt = relatedTask.UpdatedAtUtc;
            this.TaskDescription = relatedTask.Description ?? string.Empty;
        }

        base.ValidateAllProperties();
        base.CommitSnapshot();
    }

    protected async override Task<OperationResult> ApplyChangesAsync(CancellationToken cancellationToken)
    {
        OperationResult resultTask = await _taskRepository.UpdateTaskAsync(new UpdateDevTaskRequest
        {
            Id = _model.TaskId,
            Code = this.TaskCode,
            Name = this.TaskName,
            Description = this.TaskDescription,
            State = this.TaskState,
            Release = this.TaskRelease,
        }, cancellationToken)
        .ConfigureAwait(false);

        OperationResult resultEntry = await _journal.UpdateEntryAsync(new UpdateTimeEntryRequest
        {
            Id = _model.Id,
            Name = this.EntryName,
            StartedAt = this.EntryStartedAt.DateTime,
            Duration = this.EntryDuration,
        }, cancellationToken)
        .ConfigureAwait(false);

        return resultEntry && resultTask;
    }

    protected override void UndoChanges(TimeEntrySnapshot snapshot)
    {
        this.TaskName = snapshot.TaskName;
        this.TaskDescription = snapshot.TaskDescription;
        this.TaskState = snapshot.TaskState;
        this.TaskRelease = snapshot.TaskRelease;
        this.EntryName = snapshot.EntryName;
        this.EntryStartedAt = snapshot.EntryStartedAt;
        this.EntryDuration = snapshot.EntryDuration;
    }

    protected override TimeEntrySnapshot TakeSnapshot()
    {
        return new TimeEntrySnapshot()
        {
            EntryName = this.EntryName,
            EntryStartedAt = this.EntryStartedAt,
            EntryDuration = this.EntryDuration,
            TaskName = this.TaskName,
            TaskDescription = this.TaskDescription,
            TaskState = this.TaskState,
            TaskRelease = this.TaskRelease,
        };
    }

    protected override bool SnapshotEquals(TimeEntrySnapshot snapshot)
    {
        return string.Equals(snapshot.EntryName, this.EntryName, StringComparison.Ordinal)
            && string.Equals(snapshot.TaskName, this.TaskName, StringComparison.Ordinal)
            && string.Equals(snapshot.TaskDescription, this.TaskDescription, StringComparison.Ordinal)
            && snapshot.TaskState == this.TaskState
            && snapshot.TaskRelease == this.TaskRelease
            && snapshot.EntryStartedAt == this.EntryStartedAt
            && snapshot.EntryDuration == this.EntryDuration;
    }
}
