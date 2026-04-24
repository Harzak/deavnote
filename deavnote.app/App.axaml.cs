using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Targets;

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

            Task.Run(async () =>
            {
                await _serviceProvider.GetRequiredService<IDatabaseInitializer>().InitializeAsync().ConfigureAwait(false);
            });

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

        ConfigureNLog();
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

    private static void ConfigureNLog()
    {
        string appDataFolder = ApplicationEnvironment.ResolveAppDataFolder();

        NLog.Config.LoggingConfiguration config = new();

#pragma warning disable CA2000 // FileTarget lifecycle is managed by NLog's LogManager.Configuration
        FileTarget fileTarget = new("file")
        {
            FileName = Path.Combine(appDataFolder, "deavnote.${shortdate}.log"),
            MaxArchiveFiles = 30,
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${onexception:inner=|${exception:format=tostring}}",
        };
#pragma warning restore CA2000

        config.AddTarget(fileTarget);
        config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, fileTarget);

        LogManager.Configuration = config;
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null;
    }
}
