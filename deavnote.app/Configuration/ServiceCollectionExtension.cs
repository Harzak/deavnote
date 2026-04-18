using Microsoft.Extensions.DependencyInjection;

namespace deavnote.app.Configuration;

/// <summary>
/// Provides extension methods for registering application-layer services with an IServiceCollection.
/// </summary>
internal static class ServiceCollectionExtension
{
    /// <summary>
    /// Adds application-layer service dependencies, including ViewModels, to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddAppServiceDependencies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IClipboardInterop, ClipboardInterop>();
        services.AddSingleton<IViewOrchestrator, MainViewOrchestrator>();
        services.AddSingleton(provider => new Lazy<IViewOrchestrator>(provider.GetRequiredService<IViewOrchestrator>));

        return services;
    }
}
