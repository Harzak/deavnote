namespace deavnote.app.Interfaces;

internal interface ITodoHost
{
    Task OnNoteChangedAsync(Todo item);
    Task OnStateChangedAsync(Todo item);
}