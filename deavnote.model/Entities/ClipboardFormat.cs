namespace deavnote.model.Entities;

public partial class ClipboardFormat
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public EJournalContext Context { get; set; }
    public required string Template { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}