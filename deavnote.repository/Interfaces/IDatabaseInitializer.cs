namespace deavnote.repository.Interfaces;

/// <summary>
/// Initializes the application database.
/// </summary>
public interface IDatabaseInitializer
{
    /// <summary>
    /// Ensures the database schema is created and up-to-date.
    /// </summary>
    Task InitializeAsync();
}
