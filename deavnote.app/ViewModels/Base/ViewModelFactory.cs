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

    public ViewModelFactory(IJournal journal, IDevTaskRepository taskRepository, IDateProvider dateProvider, IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(taskRepository);
        ArgumentNullException.ThrowIfNull(dateProvider);
        ArgumentNullException.ThrowIfNull(dialogService);

        _journal = journal;
        _taskRepository = taskRepository;
        _dateProvider = dateProvider;
        _dialogService = dialogService;
    }

    /// <inheritdoc/>
    public JournalViewModel CreateJournalViewModel()
    {
        return new JournalViewModel(_journal, _dateProvider, this, _dialogService);
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
