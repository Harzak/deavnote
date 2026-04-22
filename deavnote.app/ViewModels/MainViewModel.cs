namespace deavnote.app.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel, IHostViewModel, IDisposable
{
    private readonly IViewOrchestrator _viewOrchestrator;

    [ObservableProperty]
    private SearchViewModel _search;

    [ObservableProperty]
    private JournalViewModel _journal;

    [ObservableProperty]
    private IEditableViewModel? _activeViewModel;

    [ObservableProperty]
    private bool _isBusy;

    public INotificationService Notifications { get; }

    public MainViewModel(
        IViewModelFactory viewModelFactory,
        IViewOrchestrator viewOrchestrator,
        INotificationService notificationService)
    {
        ArgumentNullException.ThrowIfNull(viewModelFactory);
        ArgumentNullException.ThrowIfNull(viewOrchestrator);
        ArgumentNullException.ThrowIfNull(notificationService);

        _viewOrchestrator = viewOrchestrator;

        this.Search = viewModelFactory.CreateSearchViewModel();
        this.Journal = viewModelFactory.CreateJournalViewModel();
        this.Notifications = notificationService;

        _viewOrchestrator.ActiveViewModelChanging += OnActiveViewModelChanging;
        _viewOrchestrator.ActiveViewModelChanged += OnActiveViewModelChanged;
    }

    private void OnActiveViewModelChanging(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            this.IsBusy = true;
        });
    }

    private void OnActiveViewModelChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            this.ActiveViewModel = _viewOrchestrator.ActiveViewModel;
            this.IsBusy = false;
        });
    }

    public void Dispose()
    {
        if (_viewOrchestrator != null)
        {
            _viewOrchestrator.ActiveViewModelChanging -= OnActiveViewModelChanging;
            _viewOrchestrator.ActiveViewModelChanged -= OnActiveViewModelChanged;
        }
    }
}