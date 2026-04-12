namespace deavnote.repository.Dto;

/// <summary>
/// Represents an item returned in a search result.
/// </summary>
public sealed record SearchResultItem
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required ESearchResultItemType Type { get; init; }
    public string Code { get; init; }

    public SearchResultItem()
    {
        this.Code = string.Empty;
    }
}
