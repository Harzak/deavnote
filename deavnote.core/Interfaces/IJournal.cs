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
    /// Asynchronously loads the default cursor.
    /// </summary>
    Task LoadDefaultCursorAsync();
    /// <summary>
    /// Asynchronously sets the cursors to the specified date and time.
    /// Allows for simultaneous updating of both the date and time cursors, which may trigger a refresh of the time entries collection based on the new cursor positions.
    /// </summary>
    Task SetCursorsAsync(DateTime date, TimeSpan time);
    /// <summary>
    /// Asynchronously sets the date cursor to the specified date.
    /// </summary>
    Task SetDateCursorAsync(DateTime date);
    /// <summary>
    /// Asynchronously sets the time cursor to the specified position.
    /// </summary>
    Task SetTimeCursorAsync(TimeSpan time);

    /// <summary>
    /// Occurs when the collection of time entries is modified.
    /// </summary>
    event EventHandler<TimeEntriesChangedEventArgs>? TimeEntriesChanged;
}