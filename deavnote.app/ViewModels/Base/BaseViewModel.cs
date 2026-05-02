namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides a base class for view models with support for asynchronous initialization and cleanup.
/// </summary>
internal abstract class BaseViewModel : ObservableValidator, IViewModel
{
    public abstract string Identifier { get; }

    /// <inheritdoc/>
    public virtual Task OnInitializedAsync()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnDestroyAsync()
    {
        return Task.CompletedTask;
    }
}