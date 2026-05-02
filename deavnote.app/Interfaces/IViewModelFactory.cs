[assembly: InternalsVisibleTo("deavnote.app.tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace deavnote.app.Interfaces;

/// <summary>
/// Defines a factory for creating view model instances used in the application.
/// </summary>
internal interface IViewModelFactory
{
    TimeEntryListItemViewModel CreateTimeEntryViewModel(TimeEntry timeEntry, EJournalMode journalMode);
    JournalViewModel CreateJournalViewModel();
    AddTimeEntryViewModel CreateAddTimeEntryViewModel();
    SearchViewModel CreateSearchViewModel();
    DevTaskDetailViewModel CreateDevTaskDetailViewModel(DevTask model, bool isReadonly);
    TimeEntryDetailViewModel CreateTimeEntryDetailViewModel(TimeEntry model);
};
