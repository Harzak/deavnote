using deavnote.repository.Dto;
using deavnote.utils.Results;

namespace deavnote.core.Interfaces;

/// <summary>
/// Defines functionality for managing and tracking time entries with date and time cursors.
/// </summary>
public interface IJournal
{
    /// <summary>
    /// Gets the current date cursor value.
    /// </summary>
    DateOnly DateCursor { get; }
    /// <summary>
    /// Gets the number of days to offset from <see cref="DateCursor"/>.
    /// </summary>
    int DayOffset { get; }
    /// <summary>
    /// Gets the collection of time entries corresponding to the current date and time cursors.
    /// </summary>
    IReadOnlyCollection<TimeEntry> TimeEntries { get; }
    /// <summary>
    /// Gets the default configuration for journal cursors.
    /// </summary>
    JournalConfiguration DefaultConfiguration { get; }

    /// <summary>
    /// Asynchronously loads the default cursor.
    /// </summary>
    Task LoadDefaultCursorAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Asynchronously sets journal cursors based on the specified configuration.
    /// </summary>
    Task SetCursorsAsync(JournalConfiguration configuration, CancellationToken cancellationToken = default);
    /// <summary>
    /// Asynchronously shifts the date cursor by the specified number of days.
    /// </summary>
    /// <param name="days">The number of days to shift the date cursor. Positive values move forward; negative values move backward.</param>
    Task ShiftDateCursorAsync(int days, CancellationToken cancellationToken = default);
    /// <summary>
    /// Resets the date cursor to its initial state asynchronously.
    /// </summary>
    Task ResetDateCursorAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a new time entry.
    /// </summary>
    Task<OperationResult> AddEntryAsync(AddTimeEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an existing time entry.
    /// </summary>
    Task<OperationResult> UpdateEntryAsync(UpdateTimeEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Occurs when the collection of time entries is modified.
    /// </summary>
    event EventHandler<TimeEntriesChangedEventArgs>? TimeEntriesChanged;

    event EventHandler<JournalCursorChangedEventArgs>? CursorChanged;
}