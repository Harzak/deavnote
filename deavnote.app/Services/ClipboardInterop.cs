using Avalonia.Input.Platform;

namespace deavnote.app.Services;


/// <summary>
/// Cross-platform clipboard service implementation using Avalonia's clipboard API.
/// </summary>
internal sealed class ClipboardInterop : IClipboardInterop
{
    private bool _isInitialized;
    private IClipboard? _clipboard;

    public ClipboardInterop()
    {

    }

    /// <inheritdoc />
    public async Task SetTextAsync(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            this.Initialize();
            await _clipboard!.SetTextAsync(text).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<string?> GetTextAsync()
    {
        this.Initialize();
        return await _clipboard!.TryGetTextAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ClearAsync()
    {
        this.Initialize();
        await _clipboard!.ClearAsync().ConfigureAwait(false);
    }

    private void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }
        _clipboard = GetTopLevel()?.Clipboard ?? throw new InvalidOperationException("Unable to get the top-level window for clipboard operations");
        _isInitialized = true;
    }

    private TopLevel? GetTopLevel()
    {
        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            return TopLevel.GetTopLevel(singleView.MainView);
        }

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }
}