namespace deavnote.core.EvenArgs;

/// <summary>
/// Provides data for events that signal changes to time entries, including the current date and time cursors.
/// </summary>
public sealed class TimeEntriesChangedEventArgs : EventArgs
{
    public int EntryCount { get; init; }

    public TimeEntriesChangedEventArgs(int entryCount)
    {
        this.EntryCount = entryCount;
    }
}