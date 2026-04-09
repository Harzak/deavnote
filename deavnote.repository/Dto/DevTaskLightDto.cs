namespace deavnote.repository.Dto;

/// <summary>
/// Represents a lightweight data transfer object for <see cref="DevTask"/>.
/// </summary>
public sealed record DevTaskLightDto
{
    public required int Id { get; set; }
    public required string Code { get; init; }
    public required string Name { get; init; }
}