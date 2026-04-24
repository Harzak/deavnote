namespace deavnote.repository.Interfaces;

/// <summary>
/// Provides data access methods for <see cref="TimeEntry"/> entities
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

    /// <summary>
    /// Asynchronously adds a new time entry.
    /// </summary>
    Task<OperationResult> AddTimeEntryAsync(AddTimeEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a time entry by its unique identifier.
    /// </summary>
    Task<TimeEntry?> GetEntry(int id, CancellationToken cancellationToken = default);
}