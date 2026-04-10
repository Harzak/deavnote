using CommunityToolkit.Mvvm.Messaging;
using deavnote.core.Interfaces;

namespace deavnote.app.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel, IRecipient<TimeEntrySelectedMessage>
{
    [ObservableProperty]
    public JournalViewModel _journal;

    [ObservableProperty]
    private TimeEntryDetailViewModel? _selectedTimeEntry;

    public INotificationService Notifications { get; }

    public MainViewModel(
        IViewModelFactory viewModelFactory, 
        INotificationService notificationService,
        IMessenger messenger)
    {
        ArgumentNullException.ThrowIfNull(viewModelFactory);
        ArgumentNullException.ThrowIfNull(notificationService);

        this.Journal = viewModelFactory.CreateJournalViewModel();
        this.Notifications = notificationService;

        messenger.Register(this);
    }

    public void Receive(TimeEntrySelectedMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        this.SelectedTimeEntry = new TimeEntryDetailViewModel(message.Value);
    }
}