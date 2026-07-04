namespace deavnote.app.ViewModels.Controls;

/// <summary>
/// ViewModel for the MarkdownTextBox control, managing state, mode changes, and dialog interactions.
/// </summary>
internal sealed partial class MarkdownTextBoxViewModel : BaseViewModel, IDisposable
{
    private readonly IDialogService _dialogService;
    private readonly IDebounceAction _textDebouncer;

    public override string Identifier { get; }

    [ObservableProperty]
    public partial EMarkdownTextBoxMode CurrentMode { get; set; }

    [ObservableProperty]
    public partial string Text { get; set; }

    [ObservableProperty]
    public partial string InternalTextBoxValue { get; set; }

    public MarkdownTextBoxViewModel(IDialogService dialogService, IDebounceActionFactory debounceActionFactory)
    {
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(debounceActionFactory);

        _dialogService = dialogService;

        this.Identifier = Guid.NewGuid().ToString();
        this.CurrentMode = EMarkdownTextBoxMode.View;
        this.Text = string.Empty;
        this.InternalTextBoxValue = string.Empty;

        _textDebouncer = debounceActionFactory.CreateDebounceUIAction(
            action: () => this.Text = this.InternalTextBoxValue,
            delayMs: 500);
    }

    /// <summary>
    /// Called when the internal TextBox value changes.
    /// Debounces the update to the public Text property.
    /// </summary>
    public void OnTextBoxTextChanged()
    {
        _textDebouncer.Execute();
    }

    [RelayCommand]
    private void ChangeMode(EMarkdownTextBoxMode mode)
    {
        this.CurrentMode = mode;
    }

    [RelayCommand]
    private async Task ShowCheatSheetAsync()
    {
        MarkdownCheatSheetViewModel cheatSheetViewModel = new();
        await _dialogService.ShowWindowAsync(cheatSheetViewModel, blockMainWindow: false).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _textDebouncer?.Dispose();
    }
}
