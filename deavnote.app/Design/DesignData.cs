namespace deavnote.app.Design;

/// <summary>
/// Provides pre-populated ViewModel instances for the Avalonia designer.
/// </summary>
internal static class DesignData
{
    public static ConfirmationViewModel ConfirmationViewModel { get; } = new ConfirmationViewModel(
        message: "You have unsaved changes. Would you like to save them before leaving?"
    );
}