namespace deavnote.core.Interfaces;

/// <summary>
/// Service interface for managing clipboard operations across different platforms.
/// </summary>
public interface IClipboardInterop
{
    /// <summary>
    /// Copies the specified text to the clipboard.
    /// </summary>
    Task SetTextAsync(string text);

    /// <summary>
    /// Gets the text content from the clipboard.
    /// </summary>
    Task<string?> GetTextAsync();

    /// <summary>
    /// Clears the clipboard content.
    /// </summary>
    Task ClearAsync();
}