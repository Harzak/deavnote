using deavnote.app.Controls;
using System.Windows.Input;

namespace deavnote.app.Commands;

internal sealed class ChangeMarkdownTextBoxModeCommand : ICommand
{
    private readonly MarkdownTextBox _markdownTextBox;

    public event EventHandler? CanExecuteChanged;

    public ChangeMarkdownTextBoxModeCommand(MarkdownTextBox markdownTextBox)
    {
        ArgumentNullException.ThrowIfNull(markdownTextBox);

        _markdownTextBox = markdownTextBox;
    }

    public bool CanExecute(object? parameter)
    {
        return parameter is EMarkdownTextBoxMode;
    }

    public void Execute(object? parameter)
    {
        if (parameter is EMarkdownTextBoxMode mode)
        {
            _markdownTextBox.ChangeMode(mode);
        }
    }
}