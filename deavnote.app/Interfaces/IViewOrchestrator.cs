namespace deavnote.app.Interfaces;

/// <summary>
/// Provides functionality for navigating between views and managing the active view model within the application.
/// </summary>
internal interface IViewOrchestrator
{
    /// <summary>
    /// Gets the currently active view model.
    /// </summary>
    IViewModel? ActiveViewModel { get; }

    /// <summary>
    /// Navigates asynchronously to the detail view for the specified time entry.
    /// </summary>
    Task<OperationResult> NavigateToDevTaskDetailAsync(DevTask devTask);
    /// <summary>
    /// Navigates asynchronously to the detail view for the specified development task.
    /// </summary>
    Task<OperationResult> NavigateToTimeEntryDetailAsync(TimeEntry timeEntry);

    /// <summary>
    /// Navigates to the specified view model after validating navigation guards and manages the lifecycle of the active view model.
    /// </summary>
    Task<OperationResult> NavigateToAsync(IViewModel viewModel, NavigationParameters? parameters = null);

    /// <summary>
    /// Asynchronously navigates to the to-do list view within the application.
    /// </summary>
    Task<OperationResult> NavigateToTodoListAsync();

    /// <summary>
    /// Occurs when <see cref="ActiveViewModel"/> is about to change.
    /// </summary>
    event EventHandler<ViewModelChangeEventArg>? ActiveViewModelChanging;
    /// <summary>
    /// Occurs when the active <see cref="ActiveViewModel"/> changes.
    /// </summary>
    event EventHandler<ViewModelChangeEventArg>? ActiveViewModelChanged;
}