namespace deavnote.app.Interfaces;

/// <summary>
/// Provides functionality for navigating between views and managing the active view model within the application.
/// </summary>
internal interface IViewOrchestrator
{
    /// <summary>
    /// Gets the currently active editable view model.
    /// </summary>
    IEditableViewModel? ActiveViewModel { get; }

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
    Task<OperationResult> NavigateToAsync(IEditableViewModel viewModel, NavigationParameters? parameters = null);

    /// <summary>
    /// Occurs when <see cref="ActiveViewModel"/> is about to change.
    /// </summary>
    event EventHandler<ViewModelChangeEventArg>? ActiveViewModelChanging;
    /// <summary>
    /// Occurs when the active <see cref="ActiveViewModel"/> changes.
    /// </summary>
    event EventHandler<ViewModelChangeEventArg>? ActiveViewModelChanged;
}