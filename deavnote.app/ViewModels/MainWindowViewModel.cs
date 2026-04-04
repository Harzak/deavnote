using deavnote.repository.Interfaces;
namespace deavnote.app.ViewModels;

internal sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly ITimeEntryRepository _timeEntryRepository;

    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    public MainWindowViewModel(ITimeEntryRepository timeEntryRepository)
    {
        _timeEntryRepository = timeEntryRepository;
    }

}