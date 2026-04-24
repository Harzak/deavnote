namespace deavnote.app.ViewModels.Search;

internal sealed partial class SearchViewModel : BaseViewModel, IDisposable
{
    private readonly ISearchRepository _repository;
    private readonly IViewOrchestrator _viewOrchestrator;
    private CancellationTokenSource? _searchCts;

    private static readonly TimeSpan SearchDelay = TimeSpan.FromMilliseconds(700);

    [ObservableProperty]
    private string? _searchTerms;

    [ObservableProperty]
    private SearchResultItem? _selectedItem;

    [ObservableProperty]
    private bool _hasResults;

    [ObservableProperty]
    public ObservableCollection<SearchResultItem> _searchResults;

    public SearchViewModel(
        ISearchRepository repository,
        IViewOrchestrator viewOrchestrator)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(viewOrchestrator);

        _repository = repository;
        _viewOrchestrator = viewOrchestrator;

        _searchResults = [];
    }

    partial void OnSearchTermsChanged(string? value)
    {
        ScheduleSearch(value);
    }

    partial void OnHasResultsChanged(bool value)
    {
        if (value && this.SearchResults.Count == 0)
        {
            ScheduleSearch(this.SearchTerms);
        }
    }

    private void ScheduleSearch(string? query)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();

        _ = ExecuteSearchAsync(query, _searchCts.Token);
    }

    private async Task ExecuteSearchAsync(string? query, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(SearchDelay, cancellationToken).ConfigureAwait(false);

            IReadOnlyList<SearchResultItem> results = string.IsNullOrWhiteSpace(query)
                ? await _repository.GetMostRecent(count: 15, cancellationToken).ConfigureAwait(false)
                : await _repository.Search(query, count: 15, cancellationToken).ConfigureAwait(false);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                this.SearchResults.Clear();
                foreach (SearchResultItem item in results)
                {
                    this.SearchResults.Add(item);
                }
                this.HasResults = this.SearchResults.Count > 0;
            });
        }
        catch (TaskCanceledException)
        {
            // a new search replaces a previous one
            throw;
        }
    }

    [RelayCommand]
    private void ClearSearchTerms()
    {
        this.SearchTerms = string.Empty;
        this.SearchResults.Clear();
        this.HasResults = false;
    }

    partial void OnSelectedItemChanged(SearchResultItem? value)
    {
        if (value is null)
        {
            return;
        }

        switch (value.Type)
        {
            case ESearchResultItemType.DevTask:
                Task.Run(async () =>
                {
                    await _viewOrchestrator.NavigateToDevTaskDetailAsync(value.Id).ConfigureAwait(false);
                });
                break;

            case ESearchResultItemType.TimeEntry:
                Task.Run(async () =>
                {
                    await _viewOrchestrator.NavigateToTimeEntryDetailAsync(value.Id).ConfigureAwait(false);
                });
                break;
            default:
                throw new NotImplementedException(value.Type.ToString());
        }
    }

    public void Dispose()
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = null;
    }
}
