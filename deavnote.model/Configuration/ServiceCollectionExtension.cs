using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using deavnote.model.Seed;

namespace deavnote.model.Configuration;

/// <summary>
/// Provides extension methods for registering model service dependencies with an IServiceCollection.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Adds model-related services, including the DeavnoteDbContext factory configured with the specified SQLite
    /// connection string, to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddModelServiceDependencies(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContextFactory<DeavnoteDbContext>(options => options.UseSqlite(connectionString));

        return services;
    }

    /// <summary>
    /// Applies pending migrations and seeds default data into the database.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        IDbContextFactory<DeavnoteDbContext> factory = serviceProvider.GetRequiredService<IDbContextFactory<DeavnoteDbContext>>();

        await using DeavnoteDbContext context = await factory.CreateDbContextAsync().ConfigureAwait(false);
        await context.Database.MigrateAsync().ConfigureAwait(false);

        ClipboardFormatSeeder seeder = new(context);
        await seeder.SeedAsync().ConfigureAwait(false);
    }
}

