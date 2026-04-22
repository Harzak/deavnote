namespace deavnote.app.Navigation.Context;

/// <summary>
/// Navigation context that includes parameters and metadata
/// </summary>
internal sealed class NavigationContext
{
    public NavigationParameters Parameters { get; }
    public string? SourceView { get; }
    public DateTimeOffset NavigatedAt { get; }

    public NavigationContext(NavigationParameters? parameters = null, string? sourceView = null)
    {
        Parameters = parameters ?? [];
        SourceView = sourceView;
        NavigatedAt = DateTimeOffset.UtcNow;
    }
}

