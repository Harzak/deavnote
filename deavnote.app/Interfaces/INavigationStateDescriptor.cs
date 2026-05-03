namespace deavnote.app.Interfaces;

/// <summary>
/// Describes navigation-related state exposed by a view model.
/// </summary>
internal interface INavigationStateDescriptor
{
    /// <summary>
    /// Gets a value indicating whether the source has unsaved changes that should be handled before navigation.
    /// </summary>
    bool HasUnsavedChanges { get; }

    /// <summary>
    /// Saves navigation-relevant changes before leaving the source.
    /// </summary>
    Task<OperationResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
