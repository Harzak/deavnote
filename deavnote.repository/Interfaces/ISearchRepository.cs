namespace deavnote.repository.Interfaces;

/// <summary>
/// Defines a contract for searching and retrieving search result items.
/// </summary>
public interface ISearchRepository
{
    /// <summary>
    /// Searches for items matching the specified term across development tasks, time entries, and todos.
    /// </summary>
    /// <param name="count">The maximum number of results to return. Must be greater than or equal to CATEGORY_COUNT.</param>
    /// <returns>A read-only list of search result items matching the search term.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when count is less than CATEGORY_COUNT.</exception>
    Task<IReadOnlyList<SearchResultItem>> Search(string searchTerm, int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent items from multiple categories, limited by the specified count.
    /// </summary>
    /// <param name="count">The total number of items to retrieve. Must be greater than or equal to the number of categories.</param>
    /// <returns>A read-only list of the most recent search result items.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when count is less than the required minimum per category.</exception>
    Task<IReadOnlyList<SearchResultItem>> GetMostRecent(int count = 10, CancellationToken cancellationToken = default);
}