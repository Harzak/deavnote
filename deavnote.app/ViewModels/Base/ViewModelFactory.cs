using deavnote.app.ViewModels.Search;

namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides methods to create view model instances for journals and time entries.
/// </summary>
internal sealed class ViewModelFactory : IViewModelFactory
{
    private readonly IJournal _journal;
    private readonly IDevTaskRepository _taskRepository;
    private readonly ISearchRepository _searchRepository;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IDateProvider _dateProvider;
    private readonly IDialogService _dialogService;
    private readonly Lazy<IViewOrchestrator> _viewOrchestrator;
    private readonly INotificationService _notificationService;
    private readonly IClipboardService _clipboardService;

    public ViewModelFactory(
        IJournal journal,
        IDevTaskRepository taskRepository,
        ISearchRepository searchRepository,
        ITimeEntryRepository timeEntryRepository,
        IDateProvider dateProvider,
        IDialogService dialogService,
        Lazy<IViewOrchestrator> viewOrchestrator,
        INotificationService notificationService,
        IClipboardService clipboardService)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(taskRepository);
        ArgumentNullException.ThrowIfNull(searchRepository);
        ArgumentNullException.ThrowIfNull(timeEntryRepository);
        ArgumentNullException.ThrowIfNull(dateProvider);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(viewOrchestrator);
        ArgumentNullException.ThrowIfNull(notificationService);
        ArgumentNullException.ThrowIfNull(clipboardService);

        _journal = journal;
        _taskRepository = taskRepository;
        _searchRepository = searchRepository;
        _timeEntryRepository = timeEntryRepository;
        _dateProvider = dateProvider;
        _dialogService = dialogService;
        _viewOrchestrator = viewOrchestrator;
        _notificationService = notificationService;
        _clipboardService = clipboardService;
    }

    /// <inheritdoc/>
    public JournalViewModel CreateJournalViewModel()
    {
        return new JournalViewModel(
            _journal,
            _dateProvider,
            viewModelFactory: this,
            _dialogService,
            _viewOrchestrator.Value,
            _notificationService,
            _clipboardService);
    }

    /// <inheritdoc/>
    public TimeEntryListItemViewModel CreateTimeEntryViewModel(model.Entities.TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);

        return new TimeEntryListItemViewModel(timeEntry, _clipboardService, _notificationService);
    }

    /// <inheritdoc/>
    public AddTimeEntryViewModel CreateAddTimeEntryViewModel()
    {
        return new AddTimeEntryViewModel(_taskRepository);
    }

    /// <inheritdoc/>
    public SearchViewModel CreateSearchViewModel()
    {
        return new SearchViewModel(_searchRepository, _viewOrchestrator.Value, _timeEntryRepository, _taskRepository);
    }

    /// <inheritdoc/>
    public DevTaskDetailViewModel CreateDevTaskDetailViewModel(model.Entities.DevTask model)
    {
        return new DevTaskDetailViewModel(model);
    }

    /// <inheritdoc/>
    public TimeEntryDetailViewModel CreateTimeEntryDetailViewModel(model.Entities.TimeEntry model)
    {
        return new TimeEntryDetailViewModel(model);
    }
}
