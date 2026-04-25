namespace deavnote.app.ViewModels.Search;

internal sealed partial class SearchViewModel : BaseViewModel, IDisposable
{
    private readonly ISearchRepository _repository;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IDevTaskRepository _devTaskRepository;
    private readonly IViewOrchestrator _viewOrchestrator;
    private readonly INotificationService _notification;
    private readonly DispatcherTimer _searchTimer;
    private CancellationTokenSource? _searchCts;
    private string? _pendingSearchQuery;

    private static readonly TimeSpan SearchDelay = TimeSpan.FromMilliseconds(700);

    [ObservableProperty]
    public partial string? SearchTerms { get; set; }

    [ObservableProperty]
    public partial SearchResultItem? SelectedItem { get; set; }

    [ObservableProperty]
    public partial bool HasResults { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<SearchResultItem> SearchResults { get; set; }

    public SearchViewModel(
        ISearchRepository repository,
        IViewOrchestrator viewOrchestrator,
        ITimeEntryRepository timeEntryRepository,
        IDevTaskRepository devTaskRepository,
        INotificationService notification)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(viewOrchestrator);
        ArgumentNullException.ThrowIfNull(timeEntryRepository);
        ArgumentNullException.ThrowIfNull(devTaskRepository);
        ArgumentNullException.ThrowIfNull(notification);

        _repository = repository;
        _viewOrchestrator = viewOrchestrator;
        _timeEntryRepository = timeEntryRepository;
        _devTaskRepository = devTaskRepository;
        _notification = notification;

        _searchTimer = new DispatcherTimer
        {
            Interval = SearchDelay,
        };
        _searchTimer.Tick += OnSearchTimerTick;

        this.SearchResults = [];
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
        _pendingSearchQuery = query;
        _searchTimer.Stop();
        _searchTimer.Start();
    }

    private void OnSearchTimerTick(object? sender, EventArgs e)
    {
        _searchTimer.Stop();
        StartSearch(_pendingSearchQuery);
    }

    private void StartSearch(string? query)
    {
        CancellationTokenSource? previousSearchCts = _searchCts;
        previousSearchCts?.Cancel();

        CancellationTokenSource searchCts = new();
        _searchCts = searchCts;

        _ = ExecuteSearchAsync(query, searchCts);
    }

    private async Task ExecuteSearchAsync(string? query, CancellationTokenSource searchCts)
    {
        CancellationToken cancellationToken = searchCts.Token;

        try
        {
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
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            if (ReferenceEquals(_searchCts, searchCts))
            {
                _searchCts = null;
            }

            searchCts.Dispose();
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

        OperationResult? result = null;

        switch (value.Type)
        {
            case ESearchResultItemType.DevTask:
                Task.Run(async () =>
                {
                    model.Entities.DevTask? devTask = await _devTaskRepository.GetTaskAsync(id: value.Id);
                    if (devTask != null)
                    {
                        result = await _viewOrchestrator.NavigateToDevTaskDetailAsync(devTask).ConfigureAwait(false);
                    }
                });

                break;

            case ESearchResultItemType.TimeEntry:
                Task.Run(async () =>
                {
                    model.Entities.TimeEntry? timeEntry = await _timeEntryRepository.GetEntryAsync(id: value.Id);
                    if (timeEntry != null)
                    {
                        result = await _viewOrchestrator.NavigateToTimeEntryDetailAsync(timeEntry).ConfigureAwait(false);
                    }
                });
                break;
            default:
                throw new NotImplementedException(value.Type.ToString());
        }

        if (result == null || result.IsFailed) 
        { 
            _notification.Show(Strings.SearchViewModel_Navigate_Failed, ENotificationType.Error);
        }
    }

    public void Dispose()
    {
        _searchTimer.Stop();
        _searchTimer.Tick -= OnSearchTimerTick;
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = null;
    }
}
