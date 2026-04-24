namespace deavnote.app.ViewModels.DevTask;

internal sealed partial class DevTaskDetailViewModel : DialogViewModel<OperationResult>
{
    private readonly model.Entities.DevTask _model;

    internal override string Title => _model.Name;
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
    }
}

