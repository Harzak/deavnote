using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace deavnote.app;

internal sealed partial class App : Application, IDisposable
{
    private ServiceProvider? _serviceProvider;

    internal IServiceProvider? Services => _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ServiceCollection services = new();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            _ = Task.Run(async () => await _serviceProvider.GetRequiredService<IDatabaseInitializer>().InitializeAsync().ConfigureAwait(false));

            desktop.MainWindow = new MainView
            {
                DataContext = _serviceProvider.GetRequiredService<MainViewModel>(),
            };

            desktop.ShutdownRequested += this.OnShutdownRequested;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        string connectionString = DatabasePathResolver.Resolve();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            builder.AddNLog();
        });

        services.AddUtilsServiceDependencies();
        services.AddModelServiceDependencies(connectionString);
        services.AddRepositoryServiceDependencies();
        services.AddCoreServiceDependencies();
        services.AddAppServiceDependencies();
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
