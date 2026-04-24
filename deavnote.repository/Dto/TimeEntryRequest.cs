namespace deavnote.repository.Dto;

public abstract record TimeEntryRequest
{
    public required string Name { get; init; }
    public string? WorkDone { get; init; }
    public required TimeSpan Duration { get; init; }
    public required DateTime StartedAtUtc { get; init; }
}