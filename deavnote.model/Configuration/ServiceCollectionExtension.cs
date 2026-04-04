using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddDbContextFactory<DeavnoteDbContext>(options =>options.UseSqlite(connectionString));

        return services;
    }
}
