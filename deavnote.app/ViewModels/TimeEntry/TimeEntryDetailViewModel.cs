
namespace deavnote.app.ViewModels.TimeEntry;

internal sealed partial class TimeEntryDetailViewModel
    : BaseEditableViewModel<(
        string? Name,
        string? WorkDone,
        DateTimeOffset? StartedAtUtc,
        TimeSpan? Duration)>
{

    private readonly ITimeEntryRepository _repository;
    private readonly IViewModelFactory _factory;
    private readonly int _id;
    private model.Entities.TimeEntry? _model;

    public DateTime? CreatedAtUtc => _model?.CreatedAtUtc;
    public DateTime? UpdatedAtUtc => _model?.UpdatedAtUtc;

    [ObservableProperty]
    [Required(ErrorMessage = "Name is required.")]
    [NotifyDataErrorInfo]
    private string? _name;

    [ObservableProperty]
    private string? _workDone;

    [ObservableProperty]
    [Required(ErrorMessage = "Start date is required.")]
    [NotifyDataErrorInfo]
    private DateTimeOffset? _startedAtUtc;

    [ObservableProperty]
    [Required(ErrorMessage = "Duration is required.")]
    [NotifyDataErrorInfo]
    private TimeSpan? _duration;

    [ObservableProperty]
    private DevTaskDetailViewModel? _relatedTask;

    public TimeEntryDetailViewModel(
        int id, 
        ITimeEntryRepository repository, 
        IViewModelFactory factory, 
        INotificationService notificationService)
        : base(notificationService)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(factory);
        _id = id;
        _repository = repository;
        _factory = factory;
    }

    public async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);

        _model = await _repository.GetEntryAsync(_id).ConfigureAwait(false);
        _name = _model?.Name ?? string.Empty;
        _workDone = _model?.WorkDone ?? string.Empty;
        _startedAtUtc = _model?.StartedAtUtc ?? default;
        _duration = _model?.Duration ?? default;

        if (_model?.DevTask != null)
        {
            _relatedTask = _factory.CreateDevTaskDetailViewModel(_model.DevTask.Id);
            await _relatedTask.OnInitializedAsync().ConfigureAwait(false);
        }

        base.CommitSnapshot();
        base.ValidateAllProperties();
    }

    protected override async Task<OperationResult> ApplyChangesAsync()
    {
        if (!this.StartedAtUtc.HasValue)
        {
            return OperationResult.Failure("Start date must have a value");
        }
        return await _repository.UpdateTimeEntryAsync(new UpdateTimeEntryRequest
        {
            Id = _id,
            Name = this.Name ?? string.Empty,
            WorkDone = this.WorkDone,
            StartedAtUtc = this.StartedAtUtc.Value.UtcDateTime,
            Duration = this.Duration ?? default
        })
        .ConfigureAwait(false);
    }

    protected override void UndoChanges(
        (string? Name,
         string? WorkDone,
         DateTimeOffset? StartedAtUtc,
         TimeSpan? Duration)
        snapshot)
    {
        this.Name = snapshot.Name;
        this.WorkDone = snapshot.WorkDone;
        this.StartedAtUtc = snapshot.StartedAtUtc;
        this.Duration = snapshot.Duration;
    }

    protected override (string? Name, string? WorkDone, DateTimeOffset? StartedAtUtc, TimeSpan? Duration) TakeSnapshot()
    {
        return (this.Name, this.WorkDone, this.StartedAtUtc, this.Duration);
    }

    protected override bool SnapshotEquals((string? Name, string? WorkDone, DateTimeOffset? StartedAtUtc, TimeSpan? Duration) snapshot)
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
