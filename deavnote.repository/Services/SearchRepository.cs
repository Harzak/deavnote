namespace deavnote.repository.Services;

/// <summary>
/// Provides data access methods for searching across multiple entity types (DevTask, TimeEntry, Todo)
/// </summary>
internal sealed class SearchRepository : ISearchRepository
{
    private const int CATEGORY_COUNT = 3;

    private readonly IDbContextFactory<DeavnoteDbContext> _contextFactory;

    public SearchRepository(IDbContextFactory<DeavnoteDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        _contextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SearchResultItem>> Search(string searchTerm, int count = 10, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty.", nameof(searchTerm));
        if (count < CATEGORY_COUNT)
            throw new ArgumentOutOfRangeException(nameof(count), $"Count must be greater than or equal to {CATEGORY_COUNT}.");

        List<SearchResultItem> results = [];
        int todosCount = count / 4;
        int entriesCount = count / 4;
        int tasksCount = count - entriesCount - todosCount;

        string searchPattern = $"%{searchTerm}%";

        using (DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
        {
            Task<List<SearchResultItem>> tTasks = context.DevTasks
                .Where(x => EF.Functions.Like(x.Name, searchPattern) || EF.Functions.Like(x.Code, searchPattern))
                .Select(x => new SearchResultItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = ESearchResultItemType.DevTask,
                    Code = x.Code,
                })
                .Take(tasksCount)
                .ToListAsync(cancellationToken);

            Task<List<SearchResultItem>> tEntries = context.TimeEntries
                .Where(x => EF.Functions.Like(x.Name, searchPattern))
                .Select(x => new SearchResultItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = ESearchResultItemType.TimeEntry,
                })
                .Take(entriesCount)
                .ToListAsync(cancellationToken);

            Task<List<SearchResultItem>> tTodos = context.Todos
                .Where(x => EF.Functions.Like(x.Name, searchPattern))
                .Select(x => new SearchResultItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = ESearchResultItemType.Todo,
                })
                .Take(todosCount)
                .ToListAsync(cancellationToken);

            List<SearchResultItem>[] allResults = await Task.WhenAll(tTasks, tEntries, tTodos).ConfigureAwait(false);
            results.AddRange(allResults[0]);
            results.AddRange(allResults[1]);
            results.AddRange(allResults[2]);
        }
        return results.AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SearchResultItem>> GetMostRecent(int count = 10, CancellationToken cancellationToken = default)
    {
        if (count < CATEGORY_COUNT)
            throw new ArgumentOutOfRangeException(nameof(count), $"Count must be greater than or equal to {CATEGORY_COUNT}.");

        List<SearchResultItem> results = [];

        int todosCount = count / CATEGORY_COUNT;
        int entriesCount = count / CATEGORY_COUNT;
        int tasksCount = count - entriesCount - todosCount;

        using (DeavnoteDbContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
        {
            Task<List<SearchResultItem>> tTasks = context.DevTasks
                .OrderByDescending(x => x.UpdatedAtUtc)
                .Select(x => new SearchResultItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = ESearchResultItemType.DevTask,
                    Code = x.Code,
                })
                .Take(tasksCount)
                .ToListAsync(cancellationToken);
            Task<List<SearchResultItem>> tEntries = context.TimeEntries
                .OrderByDescending(x => x.UpdatedAtUtc)
                .Select(x => new SearchResultItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = ESearchResultItemType.TimeEntry,
                })
                .Take(entriesCount)
                .ToListAsync(cancellationToken);
            Task<List<SearchResultItem>> tTodos = context.Todos
                .OrderByDescending(x => x.UpdatedAtUtc)
                .Select(x => new SearchResultItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = ESearchResultItemType.Todo,
                })
                .Take(todosCount)
                .ToListAsync(cancellationToken);

            List<SearchResultItem>[] allResults = await Task.WhenAll(tTasks, tEntries, tTodos).ConfigureAwait(false);

            results.AddRange(allResults[0]);
            results.AddRange(allResults[1]);
            results.AddRange(allResults[2]);
        }
        return results.AsReadOnly();
    }
}
