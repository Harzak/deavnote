using deavnote.core.Interfaces;

namespace deavnote.app.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel
{
    public JournalViewModel Journal { get; }

    public INotificationService Notifications { get; }

    public MainViewModel(IViewModelFactory viewModelFactory, INotificationService notificationService)
    {
        ArgumentNullException.ThrowIfNull(viewModelFactory);
        ArgumentNullException.ThrowIfNull(notificationService);

        this.Journal = viewModelFactory.CreateJournalViewModel();
        this.Notifications = notificationService;
    }
}