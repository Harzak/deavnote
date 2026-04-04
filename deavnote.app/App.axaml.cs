using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using deavnote.app.Configuration;
using deavnote.model.Configuration;
using deavnote.repository.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace deavnote.app;

internal sealed partial class App : Application, IDisposable
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            ServiceCollection services = new();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };

            desktop.ShutdownRequested += this.OnShutdownRequested;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        string connectionString = "Data Source=deavnote.db";

        services.AddModelServiceDependencies(connectionString);
        services.AddRepositoryServiceDependencies();
        services.AddAppServiceDependencies();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        DataAnnotationsValidationPlugin[] dataValidationPluginsToRemove = BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
        foreach (DataAnnotationsValidationPlugin plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        this.Dispose();
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null;
    }
}
