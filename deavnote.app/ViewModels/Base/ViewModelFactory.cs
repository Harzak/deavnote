namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides methods to create view model instances for journals and time entries.
/// </summary>
internal sealed class ViewModelFactory : IViewModelFactory
{
    private readonly IJournal _journal;
    private readonly IDateProvider _dateProvider;

    public ViewModelFactory(IJournal journal, IDateProvider dateProvider)
    {
        ArgumentNullException.ThrowIfNull(journal);
        ArgumentNullException.ThrowIfNull(dateProvider);
        
        _journal = journal;
        _dateProvider = dateProvider;
    }

    /// <inheritdoc/>
    public JournalViewModel CreateJournalViewModel()
    {
        return new JournalViewModel(_journal, _dateProvider, this);
    }

    /// <inheritdoc/>
    public TimeEntryViewModel CreateTimeEntryViewModel(TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);

        return new TimeEntryViewModel(timeEntry);
    }
}
