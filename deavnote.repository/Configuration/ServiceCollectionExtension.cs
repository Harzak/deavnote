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

        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton<ITimeEntryRepository, TimeEntryRepository>();
        services.AddSingleton<IDevTaskRepository, DevTaskRepository>();
        services.AddSingleton<ISearchRepository, SearchRepository>();
        services.AddSingleton<IClipboardFormatRepository, ClipboardFormatRepository>();
        services.AddSingleton<ITodoRepository, TodoRepository>();

        return services;
    }
}
