namespace deavnote.app.Services;

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

    public MainViewOrchestrator(IViewModelFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
        _navigationGuards = [new HasUnsavedChangeGuard()];
    }

    /// <inheritdoc/>
    public async Task NavigateToDevTaskDetailAsync(DevTask devTask)
    {
        ArgumentNullException.ThrowIfNull(devTask);

        DevTaskDetailViewModel viewModel = _factory.CreateDevTaskDetailViewModel(devTask);
        await this.NavigateToAsync(viewModel).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task NavigateToTimeEntryDetailAsync(TimeEntry timeEntry)
    {
        ArgumentNullException.ThrowIfNull(timeEntry);

        TimeEntryDetailViewModel viewModel = _factory.CreateTimeEntryDetailViewModel(timeEntry);
        await this.NavigateToAsync(viewModel).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(IEditableViewModel viewModel, NavigationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        this.ActiveViewModelChanging?.Invoke(this, EventArgs.Empty);

        NavigationContext context = new(parameters);
        foreach (INavigationGuard guard in _navigationGuards)
        {
            NavigationGuardResult result = await guard.CanNavigateAsync(ActiveViewModel, viewModel, context).ConfigureAwait(false);
            if (!result.CanNavigate)
            {
                return;
            }
        }

        if (this.ActiveViewModel != null)
        {
            await this.ActiveViewModel.OnDestroyAsync().ConfigureAwait(false);
            this.ActiveViewModel.Dispose();
        }

        this.ActiveViewModel = viewModel;
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        this.ActiveViewModelChanged?.Invoke(this, EventArgs.Empty);
    }
}