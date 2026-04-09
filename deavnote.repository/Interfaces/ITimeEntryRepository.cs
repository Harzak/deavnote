namespace deavnote.repository.Interfaces;

/// <summary>
/// Defines a repository for accessing time entry data.
/// </summary>
public interface ITimeEntryRepository
{

    /// <summary>
    /// Retrieves time entries occurring between the specified start and end dates.
    /// </summary>
    /// <param name="startDate">The start date of the range to retrieve entries for.</param>
    /// <param name="endDate">The end date of the range to retrieve entries for.</param>
    /// <returns>contains a read-only list of time entries within the specified date range.</returns>
    Task<IReadOnlyList<TimeEntry>> GetEntriesBetweenAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
}