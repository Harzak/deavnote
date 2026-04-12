namespace deavnote.app.ViewModels.Search;

internal sealed partial class SearchViewModel : BaseViewModel
{
    private readonly ISearchRepository _repository;
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IDevTaskRepository _devTaskRepository;

    public TimeSpan SearchDelay { get; } = TimeSpan.FromMilliseconds(700);
    public Func<string?, CancellationToken, Task<IEnumerable<object>>>? AsyncPopulator { get; set; }

    [ObservableProperty]
    private string? _searchTerms;

    [ObservableProperty]
    private SearchResultItem? _selectedItem;

    public SearchViewModel(
        ISearchRepository repository,
        IDialogService dialogService,
        IViewModelFactory viewModelFactory,
        ITimeEntryRepository timeEntryRepository,
        IDevTaskRepository devTaskRepository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(viewModelFactory);
        ArgumentNullException.ThrowIfNull(timeEntryRepository);
        ArgumentNullException.ThrowIfNull(devTaskRepository);

        _repository = repository;
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;
        _timeEntryRepository = timeEntryRepository;
        _devTaskRepository = devTaskRepository;

        this.AsyncPopulator = GetSearchResults;
    }

    private async Task<IEnumerable<object>> GetSearchResults(string? query, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            return await _repository.Search(query, count: 15, cancellationToken).ConfigureAwait(false);
        }
        return await _repository.GetMostRecent(count: 15, cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private void ClearSearchTerms()
    {
        this.SearchTerms = string.Empty;
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
                model.Entities.DevTask? devTask = _devTaskRepository.GetTask(id: value.Id).Result;
                if (devTask != null)
                {
                    DevTaskDetailViewModel taskVm = _viewModelFactory.CreateDevTaskDetailViewModel(devTask);
                    _dialogService.ShowWindowAsync(taskVm);
                }
                break;

            case ESearchResultItemType.TimeEntry:
                model.Entities.TimeEntry? timeEntry = _timeEntryRepository.GetEntry(id: value.Id).Result;
                if (timeEntry != null)
                {
                    TimeEntryDetailViewModel timeEntryVm = _viewModelFactory.CreateTimeEntryDetailViewModel(timeEntry);
                    _dialogService.ShowWindowAsync(timeEntryVm);
                }
                break;
            default:
                throw new NotImplementedException(value.Type.ToString());
        }
    }
}
