using System.Security.Cryptography;

namespace deavnote.app.ViewModels.DevTask;

internal sealed partial class DevTaskDetailViewModel : BaseEditableViewModel<(string Name, string Description, EDevTaskState State)>
{
    private readonly IDevTaskRepository _repository;

    private readonly model.Entities.DevTask _model;

    public DateTime? CreatedAtUtc => _model?.CreatedAtUtc;
    public DateTime? UpdatedAtUtc => _model?.UpdatedAtUtc;
    public bool IsReadonly { get; private set; }

    [ObservableProperty]
    private string _code;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private EDevTaskState _state;

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

        _code = _model.Code;
        _name = _model.Name;
        _description = _model.Description ?? string.Empty;
        _state = _model.State;

        this.IsReadonly = isReadonly;
    }

    public override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        base.CommitSnapshot();
    }

    protected override async Task<OperationResult> ApplyChangesAsync()
    {
        return await _repository.UpdateTaskAsync(new UpdateDevTaskRequest
        {
            Id = _model.Id,
            Name = this.Name,
            Code = this.Code,
            Description = this.Description,
            State = this.State
        })
        .ConfigureAwait(false);
    }

    protected override void UndoChanges((string Name, string Description, EDevTaskState State) snapshot)
    {
        this.Name = snapshot.Name;
        this.Description = snapshot.Description;
        this.State = snapshot.State;
    }

    protected override (string Name, string Description, EDevTaskState State) TakeSnapshot()
    {
        return (this.Name, this.Description, this.State);
    }

    protected override bool SnapshotEquals((string Name, string Description, EDevTaskState State) snapshot)
    {
        return snapshot.Name == this.Name
            && snapshot.Description == this.Description
            && snapshot.State == this.State;
    }
}

