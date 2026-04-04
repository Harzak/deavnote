namespace deavnote.model.Entities;

public partial class TimeEntry
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? WorkDone { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int TaskId { get; set; }
    public virtual Task Task { get; set; } = null!;
}

