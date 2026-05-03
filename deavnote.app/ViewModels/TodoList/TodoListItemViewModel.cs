namespace deavnote.app.ViewModels.TodoList;

internal sealed partial class TodoListItemViewModel : BaseViewModel
{
    private readonly ITodoHost _host;
    private readonly Todo _model;

    [ObservableProperty]
    public partial bool IsCompleted { get; set; }

    [ObservableProperty]
    public partial string? Note { get; set; }

    public override string Identifier { get; }

    public TodoListItemViewModel(Todo model, ITodoHost host)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(host);

        _model = model;
        _host = host;

        this.Identifier = model.Id.ToStringInvariant();
        this.IsCompleted = model.Status == ETodoStatus.Completed;
        this.Note = model.Note;
    }



    [RelayCommand]
    private async Task SetCompletedAsync(bool isCompleted)
    {
        _model.Status = isCompleted
            ? ETodoStatus.Completed
            : ETodoStatus.InProgress;
        await _host.OnStateChangedAsync(_model).ConfigureAwait(false);

        this.IsCompleted = isCompleted;
    }

    [RelayCommand]
    private async Task SaveNoteAsync()
    {
        if (_model.Note.EqualsOrdinalIgnoreCase(this.Note))
        {
            return;
        }

        _model.Note = this.Note;
        await _host.OnNoteChangedAsync(_model).ConfigureAwait(false);
    }
}