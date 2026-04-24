namespace deavnote.utils;

/// <summary>
/// Provides application-level environment paths.
/// </summary>
public static class ApplicationEnvironment
{
    private const string APP_NAME = "deavnote";

    /// <summary>
    /// Returns the application data directory path, creating it if needed.
    /// </summary>
    public static string ResolveAppDataFolder()
    {
        string appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            APP_NAME);

        Directory.CreateDirectory(appDataFolder);

        return appDataFolder;
    }
}
