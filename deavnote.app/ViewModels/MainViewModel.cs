using System.Reflection;

namespace deavnote.app.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel, IHostViewModel, IDisposable
{
    private readonly IViewOrchestrator _viewOrchestrator;

    public override string Identifier { get ; }

    [ObservableProperty]
    public partial SearchViewModel Search { get; set; }

    [ObservableProperty]
    public partial JournalViewModel Journal { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasContent))]
    public partial IEditableViewModel? ActiveViewModel { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasContent))]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string AppVersion { get; set; }

    [ObservableProperty]
    public partial string StoragePath { get; set; }

    public bool HasContent => !this.IsBusy && this.ActiveViewModel != null;

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

        this.Identifier = Guid.NewGuid().ToString();
        this.Search = viewModelFactory.CreateSearchViewModel();
        this.Journal = viewModelFactory.CreateJournalViewModel();
        this.Notifications = notificationService;

        // move to app configuration 
        this.AppVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToStringInvariant() ?? new Version(0, 0, 0, 0).ToStringInvariant();
        this.StoragePath = DatabasePathResolver.Resolve();

        _viewOrchestrator.ActiveViewModelChanging += OnActiveViewModelChanging;
        _viewOrchestrator.ActiveViewModelChanged += OnActiveViewModelChanged;
    }

    private void OnActiveViewModelChanging(object? sender, ViewModelChangeEventArg e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            this.IsBusy = true;
        });
    }

    private void OnActiveViewModelChanged(object? sender, ViewModelChangeEventArg e)
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