using deavnote.core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace deavnote.core.Configuration;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Adds core-layer service dependencies, including ViewModels, to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddCoreServiceDependencies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IJournal, Journal>();

        return services;
    }
}

