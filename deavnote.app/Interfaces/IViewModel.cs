namespace deavnote.app.Interfaces;

/// <summary>
/// Defines a contract for view models that require asynchronous initialization and cleanup.
/// </summary>
internal interface IViewModel
{
    /// <summary>
    /// Initializes the component asynchronously.
    /// </summary>
    Task OnInitializedAsync();

    /// <summary>
    /// Performs cleanup operations asynchronously.
    /// </summary>
    Task OnDestroyAsync();
}