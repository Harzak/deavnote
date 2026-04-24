namespace deavnote.app.Interfaces;

/// <summary>
/// A contract for editable view models with change tracking and disposal support.
/// </summary>
internal interface IEditableViewModel : IViewModel, IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the view model has unsaved changes.
    /// </summary>
    bool HasChanges { get; }
}