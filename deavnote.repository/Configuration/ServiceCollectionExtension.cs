using Microsoft.Extensions.DependencyInjection;

namespace deavnote.repository.Configuration;

/// <summary>
/// Provides extension methods for registering repository services with an IServiceCollection.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Adds repository service dependencies to the specified IServiceCollection.
    /// </summary>
    public static IServiceCollection AddRepositoryServiceDependencies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ITimeEntryRepository, TimeEntryRepository>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }
}
