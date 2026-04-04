namespace deavnote.model.Entities;

public partial class Task
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public ETaskState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}

