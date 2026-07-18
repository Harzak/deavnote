namespace deavnote.app.ViewModels.DevTask;

internal sealed partial class DevTaskDetailViewModel
    : BaseEditableViewModel<DevTaskSnapshot>
{
    private readonly IDevTaskRepository _repository;
    private readonly model.Entities.DevTask _model;

    public override string EditedElementIdentifier { get; }

    public DateTime? CreatedAt => _model?.CreatedAtUtc;
    public DateTime? UpdatedAt => _model?.UpdatedAtUtc;
    public bool IsReadonly { get; private set; }

    public string Code { get; private set; }

    [ObservableProperty]
    [Required(ErrorMessage = "Name is required.")]
    [NotifyDataErrorInfo]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial string Description { get; set; }

    [ObservableProperty]
    public partial EDevTaskState State { get; set; }

    [ObservableProperty]
    public partial Version? Release { get; set; }

    [ObservableProperty]
    public IEnumerable<model.Entities.TimeEntry> TimeEntries { get; }

    [ObservableProperty]
    public TimeSpan TotalTimeSpent { get; }

    public DevTaskDetailViewModel(
        model.Entities.DevTask model,
        bool isReadonly,
        IDevTaskRepository repository,
        INotificationService notificationService)
        : base(notificationService)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(repository);

        _model = model;
        _repository = repository;

        this.EditedElementIdentifier = _model.Id.ToString(CultureInfo.InvariantCulture);
        this.Code = _model.Code;
        this.Name = _model.Name;
        this.Description = _model.Description ?? string.Empty;
        this.State = _model.State;
        this.Release = _model.Release;
        this.IsReadonly = isReadonly;
        this.TimeEntries = _model.TimeEntries ?? Enumerable.Empty<model.Entities.TimeEntry>();
        this.TotalTimeSpent = this.TimeEntries.Sum(te => te.Duration);
    }

    public override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        base.ValidateAllProperties();
        base.CommitSnapshot();
    }

    protected override async Task<OperationResult> ApplyChangesAsync(CancellationToken cancellationToken)
    {
        return await _repository.UpdateTaskAsync(new UpdateDevTaskRequest
        {
            Id = _model.Id,
            Name = this.Name,
            Code = this.Code,
            Description = this.Description,
            State = this.State,
            Release = this.Release,
        }, cancellationToken)
        .ConfigureAwait(false);
    }

    protected override void UndoChanges(DevTaskSnapshot snapshot)
    {
        this.Name = snapshot.Name;
        this.Description = snapshot.Description;
        this.State = snapshot.State;
        this.Release = snapshot.Release;
    }

    protected override DevTaskSnapshot TakeSnapshot()
    {
        return new DevTaskSnapshot
        {
            Name = this.Name,
            Description = this.Description,
            State = this.State,
            Release = this.Release,
        };
    }

    protected override bool SnapshotEquals(DevTaskSnapshot snapshot)
    {
        return string.Equals(snapshot.Name, this.Name, StringComparison.Ordinal)
            && string.Equals(snapshot.Description, this.Description, StringComparison.Ordinal)
            && snapshot.State == this.State
            && snapshot.Release == this.Release;
    }
}

