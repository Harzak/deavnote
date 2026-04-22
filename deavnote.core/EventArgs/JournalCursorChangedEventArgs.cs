namespace deavnote.core.EventArgs;

/// <summary>
/// Provides data for the event that occurs when the journal cursor changes.
/// </summary>
public sealed class JournalCursorChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// Gets the current date and time position used for data traversal or processing.
    /// </summary>
    public DateOnly DateCursor { get; }
    /// <summary>
    /// Gets the current position in time within the timeline.
    /// </summary>
    public int DayOffset { get; }

    public JournalCursorChangedEventArgs(DateOnly dateCursor, int dayOffset)
    {
        this.DateCursor = dateCursor;
        this.DayOffset = dayOffset;
    }
}
