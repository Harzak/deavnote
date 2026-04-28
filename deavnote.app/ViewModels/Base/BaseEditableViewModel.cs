namespace deavnote.app.ViewModels.Base;

/// <summary>
/// Provides a base class for editable view models with change tracking and disposal support.
/// </summary>
internal abstract partial class BaseEditableViewModel<TSnapshot> : BaseViewModel, IEditableViewModel
{
    private static readonly CompositeFormat _saveFailedFormat = CompositeFormat.Parse(Strings.BaseEditableViewModel_Save_Failed_Format);

    private readonly INotificationService _notificationService;
    private bool _disposed;
    private TSnapshot? _snapshot;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial bool HasChanges { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial bool CanSave { get; set; }

    protected BaseEditableViewModel(INotificationService notificationService)
    {
        ArgumentNullException.ThrowIfNull(notificationService);
        _notificationService = notificationService;

        base.PropertyChanged += OnPropertyChanged;
    }

    public async Task<OperationResult> TrySaveAsync(CancellationToken cancellationToken = default)
    {
        return await Dispatcher.UIThread.InvokeAsync(async ()
            => await this.TrySaveInternalAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
    }

    protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!string.Equals(e.PropertyName, nameof(this.HasChanges), StringComparison.Ordinal))
        {
            this.HasChanges = _snapshot != null && !this.SnapshotEquals(_snapshot);
            this.CanSave = this.HasChanges && !base.HasErrors;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        _ = await this.TrySaveInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<OperationResult> TrySaveInternalAsync(CancellationToken cancellationToken)
    {
        base.ValidateAllProperties();

        if (base.HasErrors)
        {
            _notificationService.Show(Strings.BaseEditableViewModel_FixValidationErrors, ENotificationType.Warning);
            return new OperationResult().WithError(Strings.BaseEditableViewModel_FixValidationErrors);
        }

        OperationResult result = await this.ApplyChangesAsync(cancellationToken).ConfigureAwait(false);

        if (result.IsFailed)
        {
            _notificationService.Show(string.Format(CultureInfo.CurrentCulture, _saveFailedFormat, result.ErrorMessage), ENotificationType.Error);
        }
        else
        {
            _notificationService.Show(Strings.BaseEditableViewModel_Save_Success, ENotificationType.Success);
            this.CommitSnapshot();
        }

        return result;
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
        this.CanSave = false;
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
