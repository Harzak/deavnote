namespace deavnote.app.Interfaces;

/// <summary>
/// Defines a factory for creating view model instances used in the application.
/// </summary>
internal interface IViewModelFactory
{
    /// <summary>
    /// Creates a view model representation of a time entry.
    /// </summary>
    /// <param name="timeEntry">The time entry to convert.</param>
    TimeEntryViewModel CreateTimeEntryViewModel(TimeEntry timeEntry);
    /// <summary>
    /// Creates a new instance of the JournalViewModel.
    /// </summary>
    /// <returns>A JournalViewModel representing the journal data.</returns>
    JournalViewModel CreateJournalViewModel();
}
