namespace deavnote.repository.Interfaces;

/// <summary>
/// Provides data access methods for <see cref="TimeEntry"/> entities
/// </summary>
public interface ITimeEntryRepository
{

    /// <summary>
    /// Retrieves time entries occurring between the specified start and end dates.
    /// </summary>
    /// <param name="startDateUtc">The start date of the range to retrieve entries for.</param>
    /// <param name="endDateUtc">The end date of the range to retrieve entries for.</param>
    /// <returns>contains a read-only list of time entries within the specified date range.</returns>
    Task<IReadOnlyList<TimeEntry>> GetEntriesBetweenAsync(DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a new time entry.
    /// </summary>
    Task<OperationResult> AddTimeEntryAsync(AddTimeEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an existing time entry.
    /// </summary>
    Task<OperationResult> UpdateTimeEntryAsync(UpdateTimeEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a time entry by its unique identifier.
    /// </summary>
    Task<TimeEntry?> GetEntryAsync(int id, CancellationToken cancellationToken = default);
}