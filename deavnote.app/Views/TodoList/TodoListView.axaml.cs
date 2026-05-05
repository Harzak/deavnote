namespace deavnote.app.Views.TodoList;

internal sealed partial class TodoListView : UserControl
{
    public TodoListView()
    {
        InitializeComponent();
        this.AddHandler(Avalonia.Input.InputElement.LostFocusEvent, this.TodoItemNoteTextBoxLostFocus, Avalonia.Interactivity.RoutingStrategies.Bubble);
    }

    private void TodoItemNoteTextBoxLostFocus(object? sender, Avalonia.Input.FocusChangedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: TodoListItemViewModel item })
        {
            return;
        }

        item.SaveNoteCommand.Execute(null);
    }
}