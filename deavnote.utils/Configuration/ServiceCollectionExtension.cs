namespace deavnote.utils.Configuration;

/// <summary>
/// Provides extension methods for registering utils services with an IServiceCollection.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Adds utils service dependencies to the specified IServiceCollection.
    /// </summary>
    public static IServiceCollection AddUtilsServiceDependencies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IDateProvider, DateProvider>();

        return services;
    }
}

