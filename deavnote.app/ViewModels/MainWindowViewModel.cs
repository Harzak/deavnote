using deavnote.core.Interfaces;

namespace deavnote.app.ViewModels;

internal sealed partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    public JournalViewModel Journal { get; }

    public MainWindowViewModel(IJournal journal)
    {
        ArgumentNullException.ThrowIfNull(journal);

        this.Journal = new JournalViewModel(journal);
    }
}