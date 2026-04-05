using deavnote.core.Interfaces;

namespace deavnote.app.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel
{
    public JournalViewModel Journal { get; }

    public MainViewModel(IViewModelFactory viewModelFactory)
    {
        ArgumentNullException.ThrowIfNull(viewModelFactory);

        this.Journal = viewModelFactory.CreateJournalViewModel();
    }
}