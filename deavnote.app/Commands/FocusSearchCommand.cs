using System.Windows.Input;

namespace deavnote.app.Commands;

internal sealed class FocusSearchCommand : ICommand
{
    private readonly MainView _mainView;

    public event EventHandler? CanExecuteChanged;

    public FocusSearchCommand(MainView mainView)
    {
        ArgumentNullException.ThrowIfNull(mainView);

        _mainView = mainView;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _mainView.SearchView.FocusSearch();
    }
}