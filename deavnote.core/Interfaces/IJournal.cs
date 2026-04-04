namespace deavnote.core.Interfaces;

public interface IJournal
{
    DateTime DateCursor { get; }
    TimeSpan TimeCursor { get; }
    IReadOnlyCollection<TimeEntry> TimeEntries { get; }

    Task LoadDefaultCursorAsync();
    Task SetCursorsAsync(DateTime date, TimeSpan time);
    Task SetDateCursorAsync(DateTime date);
    Task SetTimeCursorAsync(TimeSpan time);

    event EventHandler<TimeEntriesChangedEventArgs>? TimeEntriesChanged;
}