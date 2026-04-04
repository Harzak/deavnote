using Microsoft.EntityFrameworkCore;

namespace deavnote.repository.Services;

/// <summary>
/// Provides data access methods for time entry entities
/// </summary>
internal sealed class TimeEntryRepository : ITimeEntryRepository
{
    private readonly DeavnoteDbContext _context;

    public TimeEntryRepository(DeavnoteDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TimeEntry>> GetEntriesForDayAsync(DateTime dateUtc, CancellationToken cancellationToken = default)
    {
        DateTime startOfDay = dateUtc.Date;
        DateTime endOfDay = dateUtc.Date.AddDays(1);

        List<TimeEntry> entries = await _context.TimeEntries
            .Where(e => e.StartedAtUtc >= startOfDay && e.StartedAtUtc < endOfDay)
            .Include(e => e.Task)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return entries.AsReadOnly();
    }
}
