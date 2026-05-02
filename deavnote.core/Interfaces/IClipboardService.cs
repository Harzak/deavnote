namespace deavnote.core.Interfaces;

public interface IClipboardService
{
    Task SetTimeEntryAsync(TimeEntry entry, CancellationToken cancellationToken = default);
    Task SetDailyTimeEntriesAsync(IEnumerable<TimeEntry> entries, CancellationToken cancellationToken = default);
    Task SetWeeklyTimeEntriesAsync(IEnumerable<TimeEntry> entries, CancellationToken cancellationToken = default);
}
