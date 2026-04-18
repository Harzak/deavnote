namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides a base class for editable view models with change tracking and disposal support.
/// </summary>
internal abstract partial class BaseEditableViewModel : BaseViewModel, IEditableViewModel
{
    private bool _disposed;

    /// <inheritdoc/>
    public bool HasChanges { get; private set; }

    public override Task OnDestroyAsync()
    {
        if (this.HasChanges && this.UserWantsToApplyChanges())
        {
            ApplyChanges();
        }
        return base.OnDestroyAsync();
    }

    private bool UserWantsToApplyChanges()
    {
        return true; // todo
    }

    protected abstract void ApplyChanges();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
