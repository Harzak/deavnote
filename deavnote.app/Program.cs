namespace deavnote.app;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Explicitly set the application UI culture. Only en-US is supported for now.
        // Setting it on the localization service propagates to CultureInfo.CurrentUICulture
        // and CultureInfo.DefaultThreadCurrentUICulture and refreshes any active bindings.
        LocalizationService.Instance.CurrentCulture = new CultureInfo("en-US");

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

