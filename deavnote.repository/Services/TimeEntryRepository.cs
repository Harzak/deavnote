using Microsoft.EntityFrameworkCore;

namespace deavnote.repository.Services;

/// <summary>
/// Provides data access methods for time entry entities
/// </summary>
internal sealed class TimeEntryRepository : ITimeEntryRepository
{
    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;

    public TimeEntryRepository(IDbContextFactory<DeavnoteDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        _contextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TimeEntry>> GetEntriesBetween(DateOnly startDateUtc, DateOnly endDateUtc, CancellationToken cancellationToken = default)
    {
        if (startDateUtc > endDateUtc)
        {
            throw new ArgumentException("Start date must be less than or equal to end date.");
        }

        DateTime startDateTime = startDateUtc.ToDateTime(TimeOnly.MinValue);
        DateTime endDateTime = endDateUtc.ToDateTime(TimeOnly.MaxValue);

        using (DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
        {
            List<TimeEntry> entries = await context.TimeEntries
              .Where(e => e.StartedAtUtc >= startDateTime && e.StartedAtUtc <= endDateTime)
              .Include(e => e.Task)
              .AsNoTracking()
              .ToListAsync(cancellationToken)
              .ConfigureAwait(false);

            return entries.AsReadOnly();
        }
    }
}