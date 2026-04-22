namespace deavnote.app.Navigation;

/// <summary>
/// Orchestrates the main view and manages navigation between view models, ensuring navigation guards are
/// respected and handling the lifecycle of the active view model.
/// </summary>
internal sealed class MainViewOrchestrator : IViewOrchestrator
{
    private readonly IViewModelFactory _factory;

    private readonly IEnumerable<INavigationGuard> _navigationGuards;

    /// <inheritdoc/>
    public IEditableViewModel? ActiveViewModel { get; private set; }

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? ActiveViewModelChanging;
    /// <inheritdoc/>
    public event EventHandler<EventArgs>? ActiveViewModelChanged;

    public MainViewOrchestrator(IViewModelFactory factory, IEnumerable<INavigationGuard> navigationGuards)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(navigationGuards);

        _factory = factory;
        _navigationGuards = navigationGuards;
    }

    /// <inheritdoc/>
    public async Task<OperationResult> NavigateToDevTaskDetailAsync(DevTask devTask)
    {
        DevTaskDetailViewModel viewModel = _factory.CreateDevTaskDetailViewModel(devTask, isReadonly: false);
        return await this.NavigateToAsync(viewModel).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<OperationResult> NavigateToTimeEntryDetailAsync(TimeEntry timeEntry)
    {
        TimeEntryDetailViewModel viewModel = _factory.CreateTimeEntryDetailViewModel(timeEntry);
        return await this.NavigateToAsync(viewModel).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<OperationResult> NavigateToAsync(IEditableViewModel viewModel, NavigationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        NavigationContext context = new(parameters);
        foreach (INavigationGuard guard in _navigationGuards)
        {
            NavigationGuardResult result = await guard.CanNavigateAsync(ActiveViewModel, viewModel, context).ConfigureAwait(false);
            if (!result.CanNavigate)
            {
                return OperationResult.Failure();
            }
        }

        this.ActiveViewModelChanging?.Invoke(this, EventArgs.Empty);

        if (this.ActiveViewModel != null)
        {
            await this.ActiveViewModel.OnDestroyAsync().ConfigureAwait(false);
            this.ActiveViewModel.Dispose();
        }

        this.ActiveViewModel = viewModel;
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        this.ActiveViewModelChanged?.Invoke(this, EventArgs.Empty);

        return OperationResult.Success();   
    }
}