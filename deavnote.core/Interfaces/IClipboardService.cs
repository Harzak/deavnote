namespace deavnote.core.Interfaces;

public interface IClipboardService
{
    Task SetDailyTimeEntryAsync(TimeEntry entry);
    Task SetDailyTimeEntriesAsync(IEnumerable<TimeEntry> entries);
    Task SetWeeklyTimeEntriesAsync(IEnumerable<TimeEntry> entries);
}
