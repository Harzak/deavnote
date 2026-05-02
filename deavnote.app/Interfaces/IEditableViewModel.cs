namespace deavnote.app.Interfaces;

/// <summary>
/// A contract for editable view models with change tracking and disposal support.
/// </summary>
internal interface IEditableViewModel : IViewModel, IDisposable
{
    /// <summary>
    /// Gets the unique identifier of the element currently being edited.
    /// </summary>
    string EditedElementIdentifier { get; }

    /// <summary>
    /// Gets a value indicating whether the view model has unsaved changes.
    /// </summary>
    bool HasChanges { get; }

    /// <summary>
    /// Try to save the current state asynchronously.
    /// </summary>
    Task<OperationResult> TrySaveAsync(CancellationToken cancellationToken = default);
}