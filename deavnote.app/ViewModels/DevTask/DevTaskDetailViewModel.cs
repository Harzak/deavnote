
namespace deavnote.app.ViewModels.DevTask;

internal sealed partial class DevTaskDetailViewModel : BaseEditableViewModel<(string Name, string Description, EDevTaskState State)>
{
    private readonly model.Entities.DevTask _model;

    public DateTime CreatedAtUtc => _model.CreatedAtUtc;
    public DateTime UpdatedAtUtc => _model.UpdatedAtUtc;

    [ObservableProperty]
    private string _code;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private EDevTaskState _state;

    public DevTaskDetailViewModel(model.Entities.DevTask model)
    {
        ArgumentNullException.ThrowIfNull(model);
        _model = model;

        _code = model.Code;
        _name = model.Name;
        _description = model.Description ?? string.Empty;
        _state = model.State;

        base.CommitSnapshot();
    }

    protected override void ApplyChanges()
    {
        throw new NotImplementedException();
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

