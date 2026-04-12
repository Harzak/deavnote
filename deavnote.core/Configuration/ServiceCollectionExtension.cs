using Microsoft.Extensions.DependencyInjection;

namespace deavnote.core.Configuration;

/// <summary>
/// Provides extension methods for registering core-layer service dependencies with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Adds core-layer service dependencies, including ViewModels, to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddCoreServiceDependencies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IJournal, Journal>();
        services.AddSingleton<IDateProvider, DateProvider>();
        services.AddSingleton<IClipboardService, JournalClipboardService>();

        return services;
    }
}