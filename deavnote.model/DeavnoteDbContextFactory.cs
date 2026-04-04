using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace deavnote.model;

internal sealed class DeavnoteDbContextFactory : IDesignTimeDbContextFactory<DeavnoteDbContext>
{
    public DeavnoteDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<DeavnoteDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite("Data Source=deavnote.db");

        return new DeavnoteDbContext(optionsBuilder.Options);
    }
}
