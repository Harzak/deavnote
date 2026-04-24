namespace deavnote.repository.Dto;

public record UpdateTimeEntryRequest : TimeEntryRequest
{
    public required int Id { get; init; }
}