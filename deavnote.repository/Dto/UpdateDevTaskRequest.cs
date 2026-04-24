namespace deavnote.repository.Dto;

public record UpdateDevTaskRequest : DevTaskRequest
{
    public required int Id { get; init; }
}