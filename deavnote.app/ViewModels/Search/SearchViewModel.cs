namespace deavnote.app.ViewModels.Search;

internal sealed partial class SearchViewModel : BaseViewModel, IDisposable
{
    private readonly ISearchRepository _repository;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IDevTaskRepository _devTaskRepository;
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
          IViewOrchestrator viewOrchestrator,
        ITimeEntryRepository timeEntryRepository,
        IDevTaskRepository devTaskRepository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(viewOrchestrator);
        ArgumentNullException.ThrowIfNull(timeEntryRepository);
        ArgumentNullException.ThrowIfNull(devTaskRepository);

        _repository = repository;
        _viewOrchestrator = viewOrchestrator;
        _timeEntryRepository = timeEntryRepository;
        _devTaskRepository = devTaskRepository;

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

            await Dispatcher.UIThread.InvokeAsync(() =>
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
                model.Entities.DevTask? devTask = _devTaskRepository.GetTaskAsync(id: value.Id).Result;
                if (devTask != null)
                {
                    Task.Run(async () =>
                    {
                        await _viewOrchestrator.NavigateToDevTaskDetailAsync(devTask).ConfigureAwait(false);
                    });
                }

                break;

            case ESearchResultItemType.TimeEntry:
                model.Entities.TimeEntry? timeEntry = _timeEntryRepository.GetEntryAsync(id: value.Id).Result;
                if (timeEntry != null)
                {
                    Task.Run(async () =>
                    {
                        await _viewOrchestrator.NavigateToTimeEntryDetailAsync(timeEntry).ConfigureAwait(false);
                    });
                }
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
