namespace deavnote.model.Entities;

/// <summary>
/// Represents a development task with associated metadata, state, and time entries.
/// </summary>
public partial class DevTask
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public EDevTaskState State { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}

