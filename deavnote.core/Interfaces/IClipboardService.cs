namespace deavnote.core.Interfaces;

/// <summary>
/// Service responsible for formatting time entries and setting them to the clipboard -
/// based on user-defined templates for different journal modes (single entry, daily, weekly).
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Sets the clipboard text to a formatted representation of a single time entry.
    /// </summary>
    Task SetTimeEntryAsync(TimeEntry entry, CancellationToken cancellationToken = default);
    /// <summary>
    /// Sets the clipboard text to a formatted representation of multiple time entries for a single day.
    /// </summary>
    Task SetDailyTimeEntriesAsync(IEnumerable<TimeEntry> entries, CancellationToken cancellationToken = default);
    /// <summary>
    /// Sets the clipboard text to a formatted representation of multiple time entries for a week.
    /// </summary>
    Task SetWeeklyTimeEntriesAsync(IEnumerable<TimeEntry> entries, CancellationToken cancellationToken = default);
}
