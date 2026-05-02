namespace deavnote.model.Seed;

/// <summary>
/// Seeds default <see cref="ClipboardFormat"/> records.
/// </summary>
public sealed class ClipboardFormatSeeder
{
    private static readonly ClipboardFormat[] _defaults =
    [
         new ClipboardFormat
        {
            Id = 1,
            Name = "Single Daily Default",
            Context = EJournalMode.TimeEntry,
            Template = "{TaskCode}-{TaskName}: {EntryName}",
            IsDefault = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        },
        new ClipboardFormat
        {
            Id = 2,
            Name = "Multiple Daily Default",
            Context = EJournalMode.Day,
            Template = "{TaskCode}-{TaskName}: {EntryName}\n-{WorkDone}",
            IsDefault = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        },
        new ClipboardFormat
        {
            Id = 3,
            Name = "Weekly Default",
            Context = EJournalMode.Week,
            Template = "{TaskCode}-{TaskName}: {EntryName}",
            IsDefault = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        }
    ];

    private readonly DeavnoteDbContext _context;

    public ClipboardFormatSeeder(DeavnoteDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task SeedAsync()
    {
        bool hasData = await _context.ClipboardFormats.AnyAsync().ConfigureAwait(false);
        if (hasData)
        {
            return;
        }

        await _context.ClipboardFormats.AddRangeAsync(_defaults).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
