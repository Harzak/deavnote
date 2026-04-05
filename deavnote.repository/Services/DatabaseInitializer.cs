namespace deavnote.repository.Services;

/// <summary>
/// Ensures the application database schema is created on startup.
/// </summary>
internal sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IDbContextFactory<DeavnoteDbContext> _factory;

    public DatabaseInitializer(IDbContextFactory<DeavnoteDbContext> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <inheritdoc/>
    public void Initialize()
    {
        using DeavnoteDbContext context = _factory.CreateDbContext();
        context.Database.EnsureCreated();
    }
}
