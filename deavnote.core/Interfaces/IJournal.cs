using deavnote.core.Services;

namespace deavnote.core.Interfaces;

/// <summary>
/// Defines functionality for managing and tracking time entries with date and time cursors.
/// </summary>
public interface IJournal
{
    /// <summary>
    /// Gets the current date and time cursor position.
    /// </summary>
    DateTime DateCursor { get; }
    /// <summary>
    /// Gets the current position in time.
    /// </summary>
    TimeSpan TimeCursor { get; }
    /// <summary>
    /// Gets the collection of time entries corresponding to the current date and time cursors.
    /// </summary>
    IReadOnlyCollection<TimeEntry> TimeEntries { get; }
    /// <summary>
    /// Gets the default configuration for journal cursors.
    /// </summary>
    JournalCursorsConfiguration DefaultConfiguration { get; }

    /// <summary>
    /// Asynchronously loads the default cursor.
    /// </summary>
    Task LoadDefaultCursorAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Asynchronously sets journal cursors based on the specified configuration.
    /// </summary>
    Task SetCursorsAsync(JournalCursorsConfiguration configuration, CancellationToken cancellationToken = default);
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
    /// Occurs when the collection of time entries is modified.
    /// </summary>
    event EventHandler<TimeEntriesChangedEventArgs>? TimeEntriesChanged;
}