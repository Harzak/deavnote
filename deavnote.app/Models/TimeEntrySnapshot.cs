namespace deavnote.app.Models;

internal sealed record TimeEntrySnapshot
{
    internal string EntryName { get; init; } = string.Empty;
    internal DateTimeOffset EntryStartedAt { get; init; }
    internal TimeSpan EntryDuration { get; init; }
    internal string TaskName { get; init; } = string.Empty;
    internal string TaskDescription { get; init; } = string.Empty;
    internal EDevTaskState TaskState { get; init; }
    internal Version? TaskRelease { get; init; }
}
