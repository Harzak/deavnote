namespace deavnote.repository.Services;

/// <summary>
/// Provides data access methods for <see cref="ClipboardFormat"/> entities
/// </summary>
internal sealed class ClipboardFormatRepository : IClipboardFormatRepository
{
    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;
    private readonly ILogger<ClipboardFormatRepository> _logger;

    public ClipboardFormatRepository(IDbContextFactory<DeavnoteDbContext> contextFactory, ILogger<ClipboardFormatRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        ArgumentNullException.ThrowIfNull(logger);

        _contextFactory = contextFactory;
        _logger=logger;
    }

    /// <inheritdoc/>
    public async Task<string> GetTemplateAsync(EJournalMode context, CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        string? format = await dbContext.ClipboardFormats
            .Where(x => x.Context == context && x.IsActive)
            .Select(x => x.Template)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return format ?? throw new InvalidOperationException($"No default clipboard format found for context '{context}'.");
    }

    /// <inheritdoc/>
    public async Task<OperationResult> SetTemplateAsync(EJournalMode context, string format, CancellationToken cancellationToken = default)
    {
        using DeavnoteDbContext dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        ClipboardFormat? clipboardFormat = await dbContext.ClipboardFormats
            .Where(x => x.Context == context && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (clipboardFormat == null)
        {
            return OperationResult.Failure($"Clipboard template for context '{context}' not found.");
        }

        clipboardFormat.Template = format;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            ClipboardFormatLogMessages.LogFailedToUpdateClipboardFormat(_logger, context, ex);
            return OperationResult.Failure($"Failed to update clipboard format for context '{context}'.");
        }

        return OperationResult.Success();
    }
}
