namespace deavnote.app.DesignData;

/// <summary>
/// Provides pre-populated ViewModel instances for the Avalonia designer.
/// </summary>
internal static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel { get; } = new(null!)
    {
        Greeting = "Hello, Designer!"
    };
}
