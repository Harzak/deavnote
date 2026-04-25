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
    public async Task InitializeAsync()
    {
        using DeavnoteDbContext context = await _factory.CreateDbContextAsync().ConfigureAwait(false);
        await context.Database.MigrateAsync().ConfigureAwait(false);

        ClipboardFormatSeeder seeder = new(context);
        await seeder.SeedAsync().ConfigureAwait(false);
    }
}
