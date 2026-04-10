namespace deavnote.repository.Dto;

/// <summary>
/// Represents a request to add a time entry, with explicit link (or creation) to a task.
/// </summary>
public abstract record AddTimeEntryRequest
{
    public required string Name { get; init; }
    public string? WorkDone { get; init; }
    public required TimeSpan Duration { get; init; }
    public required DateTime StartedAtUtc { get; init; }

    /// <summary>
    /// Links the time entry to an existing task by ID.
    /// </summary>
    public sealed record ForExistingTask : AddTimeEntryRequest
    {
        public required int TaskId { get; init; }
    }

    /// <summary>
    /// Creates a new task alongside the time entry.
    /// </summary>
    public sealed record ForNewTask : AddTimeEntryRequest
    {
        public required string TaskCode { get; init; }
        public required string TaskName { get; init; }
    }
}
