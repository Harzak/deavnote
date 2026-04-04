namespace deavnote.repository.Interfaces;

/// <summary>
/// Defines a repository for accessing time entry data.
/// </summary>
public interface ITimeEntryRepository
{
    /// <summary>
    /// Asynchronously retrieves time entries for the specified UTC date.
    /// </summary>
    /// <param name="dateUtc">The date in UTC for which to retrieve time entries.</param>
    /// <returns>The task result contains a read-only list of time entries for the specified date.</returns>
    Task<IReadOnlyList<TimeEntry>> GetEntriesForDayAsync(DateTime dateUtc, CancellationToken cancellationToken = default);
}