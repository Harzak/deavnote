namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides a base class for editable view models with change tracking and disposal support.
/// </summary>
internal abstract partial class BaseEditableViewModel<TSnapshot> : BaseViewModel, IEditableViewModel
{
    private readonly INotificationService _notificationService;
    private bool _disposed;
    private TSnapshot? _snapshot;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool _hasChanges;

    protected BaseEditableViewModel(INotificationService notificationService)
    {
        ArgumentNullException.ThrowIfNull(notificationService);
        _notificationService = notificationService;

        base.PropertyChanged += OnPropertyChanged;
    }

    protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(this.HasChanges))
        {
            this.HasChanges = _snapshot != null && !this.SnapshotEquals(_snapshot);
        }
    }

    [RelayCommand(CanExecute = nameof(HasChanges))]
    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        base.ValidateAllProperties();
        if (base.HasErrors)
        {
            _notificationService.Show("Please fix the validation errors before saving.", ENotificationType.Warning);
            return;
        }
        OperationResult result = await this.ApplyChangesAsync(cancellationToken).ConfigureAwait(false);
        if (result.IsFailed)
        {
            _notificationService.Show("An error occurred while saving changes." + result.ErrorMessage, ENotificationType.Error);
        }
        else
        {
            _notificationService.Show("Changes saved successfully.", ENotificationType.Success);
            this.CommitSnapshot();
        }
    }

    [RelayCommand(CanExecute = nameof(HasChanges))]
    private void Cancel()
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
    protected abstract Task<OperationResult> ApplyChangesAsync(CancellationToken cancellationToken);

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
        base.PropertyChanged -= OnPropertyChanged;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
