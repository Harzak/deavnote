namespace deavnote.repository.Interfaces;

/// <summary>
/// Provides data access methods for <see cref="ClipboardFormat"/> entities
/// </summary>
public interface IClipboardFormatRepository
{
    /// <summary>
    /// Asynchronously retrieves the active clipboard template for the specified journal context.
    /// </summary>
    /// <param name="context">The journal context for which to retrieve the clipboard template.</param>
    Task<string> GetTemplateAsync(EJournalMode context, CancellationToken cancellationToken = default);
}