namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides methods to create view model instances for journals and time entries.
/// </summary>
internal sealed class ViewModelFactory : IViewModelFactory
{
    private readonly IJournal _journal;
    private readonly IDevTaskRepository _taskRepository;
    private readonly IDateProvider _dateProvider;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly IMessenger _messenger;

    public ViewModelFactory(
        IJournal journal,
        IDevTaskRepository taskRepository,
        IDateProvider dateProvider,
        IDialogService dialogService,
        INotificationService notificationService,
        IMessenger messenger)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(taskRepository);
        ArgumentNullException.ThrowIfNull(dateProvider);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(notificationService);
        ArgumentNullException.ThrowIfNull(messenger);

        _journal = journal;
        _taskRepository = taskRepository;
        _dateProvider = dateProvider;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _messenger = messenger;
    }

    /// <inheritdoc/>
    public JournalViewModel CreateJournalViewModel()
    {
        return new JournalViewModel(_journal, _dateProvider, viewModelFactory: this, _dialogService, _notificationService, _messenger);
    }

    /// <inheritdoc/>
    public TimeEntryListItemViewModel CreateTimeEntryViewModel(model.Entities.TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);

        return new TimeEntryListItemViewModel(timeEntry);
    }

    /// <inheritdoc/>
    public AddTimeEntryViewModel CreateAddTimeEntryViewModel()
    {
        return new AddTimeEntryViewModel(_taskRepository);
    }
}
