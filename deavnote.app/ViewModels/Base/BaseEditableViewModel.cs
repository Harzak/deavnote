using FluentIcons.Common.Internals;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;

namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides a base class for editable view models with change tracking and disposal support.
/// </summary>
internal abstract partial class BaseEditableViewModel<TSnapshot> : BaseViewModel, IEditableViewModel
{
    private bool _disposed;
    private TSnapshot? _snapshot;

    /// <inheritdoc/>
    public bool HasChanges { get; private set; }

    protected BaseEditableViewModel()
    {
        PropertyChanged += OnPropertyChanged;
    }

    protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(this.HasChanges))
        {
            this.HasChanges = _snapshot != null && !this.SnapshotEquals(_snapshot);
        }
    }

    public override Task OnDestroyAsync()
    {
        return base.OnDestroyAsync();
    }

    [RelayCommand]
    private void SaveCommand()
    {
        this.ApplyChanges();
        this.CommitSnapshot();
    }

    [RelayCommand]
    private void CancelCommand()
    {
        if (_snapshot != null)
        {
            this.UndoChanges(_snapshot);
        }
        this.CommitSnapshot();
    }

    /// <summary>
    /// Applies the changes made to the object's state.
    /// </summary>
    protected abstract void ApplyChanges();

    /// <summary>
    /// Reverts any changes made to the current state.
    /// </summary>
    protected abstract void UndoChanges(TSnapshot snapshot);

    /// <summary>
    /// Commits the current object's state as a snapshot.
    /// </summary>
    protected abstract TSnapshot TakeSnapshot();

    protected void CommitSnapshot()
    {
        _snapshot = this.TakeSnapshot();
        this.HasChanges = false;
    }

    protected abstract bool SnapshotEquals(TSnapshot snapshot);

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
