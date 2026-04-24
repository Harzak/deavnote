namespace deavnote.app.ViewModels.DevTask;

internal sealed partial class DevTaskDetailViewModel : BaseEditableViewModel<(string? Name, string? Description, EDevTaskState State)>
{
    private readonly IDevTaskRepository _repository;
    private readonly int _id;

    private model.Entities.DevTask? _model;

    public DateTime? CreatedAtUtc => _model?.CreatedAtUtc;
    public DateTime? UpdatedAtUtc => _model?.UpdatedAtUtc;

    [ObservableProperty]
    private string? _code;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private EDevTaskState _state;

    public DevTaskDetailViewModel(int id, IDevTaskRepository repository, INotificationService notificationService)
        : base(notificationService)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _id = id;
        _repository = repository;
    }

    public override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);

        _model = await _repository.GetTaskAsync(_id).ConfigureAwait(false);
        _code = _model?.Code ?? string.Empty;
        _name = _model?.Name ?? string.Empty;
        _description = _model?.Description ?? string.Empty;
        _state = _model?.State ?? EDevTaskState.Unknown;

        base.CommitSnapshot();
    }

    protected override async Task<OperationResult> ApplyChangesAsync()
    {
        return await _repository.UpdateTaskAsync(new UpdateDevTaskRequest
        {
            Id = _id,
            Name = this.Name ?? string.Empty,
            Code = this.Code ?? string.Empty,
            Description = this.Description,
            State = this.State
        })
        .ConfigureAwait(false);
    }

    protected override void UndoChanges((string? Name, string? Description, EDevTaskState State) snapshot)
    {
        this.Name = snapshot.Name;
        this.Description = snapshot.Description;
        this.State = snapshot.State;
    }

    protected override (string? Name, string? Description, EDevTaskState State) TakeSnapshot()
    {
        return (this.Name, this.Description, this.State);
    }

    protected override bool SnapshotEquals((string? Name, string? Description, EDevTaskState State) snapshot)
    {
        return snapshot.Name == this.Name
            && snapshot.Description == this.Description
            && snapshot.State == this.State;
    }
}

