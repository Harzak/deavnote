namespace deavnote.repository.Dto;

public abstract record DevTaskRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public EDevTaskState State { get; init; }
}

public record UpdateDevTaskRequest : DevTaskRequest
{
    public required int Id { get; init; }
}
