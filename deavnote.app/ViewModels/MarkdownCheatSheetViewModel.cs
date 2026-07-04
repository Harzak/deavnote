namespace deavnote.app.ViewModels;

/// <summary>
/// ViewModel for the Markdown CheatSheet help dialog.
/// </summary>
internal sealed partial class MarkdownCheatSheetViewModel : DialogViewModel<bool>
{
    internal override string Title => "Markdown Cheat Sheet";

    [RelayCommand]
    private void Close()
    {
        this.Close(result: true);
    }
}
