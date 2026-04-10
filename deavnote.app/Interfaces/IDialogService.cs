namespace deavnote.app.Interfaces;

/// <summary>
/// Defines a service for displaying modal dialog windows.
/// </summary>
internal interface IDialogService
{
    /// <summary>
    /// Opens a modal dialog window for the specified view model and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the dialog.</typeparam>
    /// <param name="viewModel">The view model that drives the dialog content.</param>
    Task<TResult?> ShowWindowAsync<TResult>(DialogViewModel<TResult> viewModel);
}
