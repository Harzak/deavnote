namespace deavnote.app.Navigation.States;

/// <summary>
/// Navigation state descriptor for editable view models.
/// </summary>
internal sealed class EditableNavigationStateDescriptor : INavigationStateDescriptor
{
    private readonly Func<bool> _hasUnsavedChanges;
    private readonly Func<CancellationToken, Task<OperationResult>> _saveChangesAsync;

    public bool HasUnsavedChanges => _hasUnsavedChanges();

    public EditableNavigationStateDescriptor(
        Func<bool> hasUnsavedChanges,
        Func<CancellationToken, Task<OperationResult>> saveChangesAsync)
    {
        ArgumentNullException.ThrowIfNull(hasUnsavedChanges);
        ArgumentNullException.ThrowIfNull(saveChangesAsync);

        _hasUnsavedChanges = hasUnsavedChanges;
        _saveChangesAsync = saveChangesAsync;
    }

    public async Task<OperationResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _saveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
