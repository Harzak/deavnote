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

    public async Task<IReadOnlyList<TimeEntry>> GetEntriesBetween(DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken = default)
    {
        if(startDateUtc > endDateUtc)
        {
            throw new ArgumentException("Start date must be less than or equal to end date.");
        }

        List<TimeEntry> entries = await _context.TimeEntries
            .Where(e => e.StartedAtUtc >= startDateUtc && e.StartedAtUtc < endDateUtc)
            .Include(e => e.Task)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return entries.AsReadOnly();
    }
}
