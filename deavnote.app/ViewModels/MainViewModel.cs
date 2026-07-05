namespace deavnote.app.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel, IHostViewModel, IDisposable
{
    private readonly IViewOrchestrator _viewOrchestrator;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IDialogService _dialogService;

    public override string Identifier { get; }

    [ObservableProperty]
    public partial SearchViewModel Search { get; set; }

    [ObservableProperty]
    public partial JournalViewModel Journal { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasContent))]
    public partial IViewModel? ActiveViewModel { get; set; }

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
        INotificationService notificationService,
        IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(viewModelFactory);
        ArgumentNullException.ThrowIfNull(viewOrchestrator);
        ArgumentNullException.ThrowIfNull(notificationService);
        ArgumentNullException.ThrowIfNull(dialogService);

        _viewOrchestrator = viewOrchestrator;
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;

        this.Identifier = Guid.NewGuid().ToString();
        this.Search = _viewModelFactory.CreateSearchViewModel();
        this.Journal = _viewModelFactory.CreateJournalViewModel();
        this.Notifications = notificationService;

        // move to app configuration 
        this.AppVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToStringInvariant() ?? new Version(0, 0, 0, 0).ToStringInvariant();
        this.StoragePath = ApplicationEnvironment.ResolveAppDataFolder();

        _viewOrchestrator.ActiveViewModelChanging += OnActiveViewModelChanging;
        _viewOrchestrator.ActiveViewModelChanged += OnActiveViewModelChanged;
    }

    [RelayCommand]
    private async Task NavigateTodoList()
    {
        await _viewOrchestrator.NavigateToTodoListAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task OpenSettings()
    {
        SettingsViewModel vm = _viewModelFactory.CreateSettingsViewModel();
        await _dialogService.ShowWindowAsync(vm).ConfigureAwait(false);
    }

    private void OnActiveViewModelChanging(object? sender, ViewModelChangeEventArg e)
    {
        Dispatcher.UIThread.Invoke(() => this.IsBusy = true);
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