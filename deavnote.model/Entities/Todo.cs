namespace deavnote.model.Entities;

/// <summary>
/// Represents a to-do item with identifying information, description, and timestamps.
/// </summary>
public partial class Todo
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

