namespace deavnote.repository;

/// <summary>
/// Resolves the SQLite connection string from the OS application data folder.
/// </summary>
public static class DatabasePathResolver
{
    private const string APP_NAME = "deavnote";
    private const string DATABASE_NAME = "deavnote.db";
    private static readonly CompositeFormat SQLITE_CONNECTION_FORMAT = CompositeFormat.Parse("Data Source={0}");

    /// <summary>
    /// Builds and returns the SQLite connection string, creating the application data directory if needed.
    /// </summary>
    public static string Resolve()
    {
        string appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            APP_NAME);

        Directory.CreateDirectory(appDataFolder);

        string dbPath = Path.Combine(appDataFolder, DATABASE_NAME);

        return string.Format(CultureInfo.InvariantCulture, SQLITE_CONNECTION_FORMAT, dbPath);
    }
}
