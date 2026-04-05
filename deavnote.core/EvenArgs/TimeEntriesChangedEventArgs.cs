namespace deavnote.core.EvenArgs;

/// <summary>
/// Provides data for events that signal changes to time entries, including the current date and time cursors.
/// </summary>
public sealed class TimeEntriesChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the current date and time position used for data traversal or processing.
    /// </summary>
    /// 
    public DateTime DateCursor { get; }
    /// <summary>
    /// Gets the current position in time within the timeline.
    /// </summary>
    public TimeSpan TimeCursor { get; }

    public TimeEntriesChangedEventArgs(DateTime dateCursor, TimeSpan timeCursor)
    {
        this.DateCursor = dateCursor;
        this.TimeCursor = timeCursor;
    }
}