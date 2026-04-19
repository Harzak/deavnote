using deavnote.app.ViewModels.Search;
using Microsoft.Extensions.DependencyInjection;

namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides methods to create view model instances using the service provider for dependency resolution.
/// </summary>
internal sealed class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public JournalViewModel CreateJournalViewModel()
    {
        return ActivatorUtilities.CreateInstance<JournalViewModel>(_serviceProvider);
    }

    /// <inheritdoc/>
    public TimeEntryListItemViewModel CreateTimeEntryViewModel(model.Entities.TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);
        return ActivatorUtilities.CreateInstance<TimeEntryListItemViewModel>(_serviceProvider, timeEntry);
    }

    /// <inheritdoc/>
    public AddTimeEntryViewModel CreateAddTimeEntryViewModel()
    {
        return ActivatorUtilities.CreateInstance<AddTimeEntryViewModel>(_serviceProvider);
    }

    /// <inheritdoc/>
    public SearchViewModel CreateSearchViewModel()
    {
        return ActivatorUtilities.CreateInstance<SearchViewModel>(_serviceProvider);
    }

    /// <inheritdoc/>
    public DevTaskDetailViewModel CreateDevTaskDetailViewModel(int id)
    {
        return ActivatorUtilities.CreateInstance<DevTaskDetailViewModel>(_serviceProvider, id);
    }

    /// <inheritdoc/>
    public TimeEntryDetailViewModel CreateTimeEntryDetailViewModel(int id)
    {
        return ActivatorUtilities.CreateInstance<TimeEntryDetailViewModel>(_serviceProvider, id);
    }
}
