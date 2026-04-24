namespace deavnote.core.EventArgs;

/// <summary>
/// Provides data for events that signal changes to time entries, including the current date and time cursors.
/// </summary>
public sealed class TimeEntriesChangedEventArgs : System.EventArgs
{
    public int EntryCount { get; init; }

    public TimeEntriesChangedEventArgs(int entryCount)
    {
        this.EntryCount = entryCount;
    }
}