using Microsoft.EntityFrameworkCore.Design;
using System.Globalization;
using System.Text;

namespace deavnote.model.Configuration;

/// <summary>
/// Provides a design-time factory for creating instances of DeavnoteDbContext for use with Entity Framework Core tools.
/// </summary>
/// <remarks>Used by Entity Framework Core tooling to configure and instantiate DeavnoteDbContext at design time,
/// such as during migrations.</remarks>
internal sealed class DeavnoteDbContextFactory : IDesignTimeDbContextFactory<DeavnoteDbContext>
{
    private const string DATABASE_NAME = "deavnote.db";
    private readonly CompositeFormat SQLITE_CONNECTION_FORMAT = CompositeFormat.Parse("Data Source={0}");

    /// <summary>
    /// Creates a new instance of DeavnoteDbContext configured to use a SQLite database.
    /// </summary>
    public DeavnoteDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<DeavnoteDbContext> optionsBuilder = new();
        string connectionString = string.Format(CultureInfo.InvariantCulture, SQLITE_CONNECTION_FORMAT, DATABASE_NAME);
        optionsBuilder.UseSqlite(connectionString);

        return new DeavnoteDbContext(optionsBuilder.Options);
    }
}
