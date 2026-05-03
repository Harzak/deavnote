namespace deavnote.app.Interfaces;

/// <summary>
/// Defines a contract for view models that require asynchronous initialization and cleanup.
/// </summary>
    internal interface IViewModel
{
    string Identifier { get; }

    /// <summary>
    /// Gets navigation-related state exposed by this view model.
    /// </summary>
    INavigationStateDescriptor NavigationState { get; }

    /// <summary>
    /// Initializes the component asynchronously.
    /// </summary>
    Task OnInitializedAsync();

    /// <summary>
    /// Performs cleanup operations asynchronously.
    /// </summary>
    Task OnDestroyAsync();
}