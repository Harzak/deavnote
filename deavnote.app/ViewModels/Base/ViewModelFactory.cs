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

    public ViewModelFactory(
        IJournal journal,
        IDevTaskRepository taskRepository,
        IDateProvider dateProvider,
        IDialogService dialogService,
        INotificationService notificationService)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(taskRepository);
        ArgumentNullException.ThrowIfNull(dateProvider);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(notificationService);

        _journal = journal;
        _taskRepository = taskRepository;
        _dateProvider = dateProvider;
        _dialogService = dialogService;
        _notificationService = notificationService;
    }

    /// <inheritdoc/>
    public JournalViewModel CreateJournalViewModel()
    {
        return new JournalViewModel(_journal, _dateProvider, this, _dialogService, _notificationService);
    }

    /// <inheritdoc/>
    public TimeEntryViewModel CreateTimeEntryViewModel(TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);

        return new TimeEntryViewModel(timeEntry);
    }

    /// <inheritdoc/>
    public AddTimeEntryViewModel CreateAddTimeEntryViewModel()
    {
        return new AddTimeEntryViewModel(_taskRepository);
    }
}
