namespace deavnote.core.EvenArgs;
public sealed class TimeEntriesChangedEventArgs : EventArgs
{
    public DateTime DateCursor { get; }
    public TimeSpan TimeCursor { get; }

    public TimeEntriesChangedEventArgs(DateTime dateCursor, TimeSpan timeCursor)
    {
        this.DateCursor = dateCursor;
        this.TimeCursor = timeCursor;
    }
}