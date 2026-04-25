namespace deavnote.repository.Services;

/// <summary>
/// Provides data access methods for <see cref="ClipboardFormat"/> entities
/// </summary>
internal sealed class ClipboardFormatRepository : IClipboardFormatRepository
{
    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;

    public ClipboardFormatRepository(IDbContextFactory<DeavnoteDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        _contextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public async Task<string> GetTemplateAsync(EJournalContext context, CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        string? format = await dbContext.ClipboardFormats
            .Where(x => x.Context == context && x.IsActive)
            .Select(x => x.Template)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return format ?? throw new InvalidOperationException($"No default clipboard format found for context '{context}'.");
    }
}
