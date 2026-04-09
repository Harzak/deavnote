namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Base class for view models that back a modal dialog window.
/// Exposes a close action the view model uses to dismiss the window with a result.
/// </summary>
/// <typeparam name="TResult">The type of result produced by the dialog.</typeparam>
internal abstract class DialogViewModel<TResult> : BaseViewModel
{
    internal abstract string Title { get; }
    internal Action<TResult?>? CloseDialog { get; set; }

    protected void Close(TResult? result = default) => CloseDialog?.Invoke(result);
}
