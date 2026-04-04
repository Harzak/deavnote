namespace deavnote.model.Entities;

/// <summary>
/// Represents a record of time spent on a specific task, including details such as duration, description, and timestamps.
/// </summary>
public partial class TimeEntry
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? WorkDone { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public int TaskId { get; set; }
    public virtual DevTask Task { get; set; } = null!;
}

