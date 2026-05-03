namespace deavnote.app.Interfaces;

/// <summary>
/// A contract for editable view models with disposal support.
/// </summary>
internal interface IEditableViewModel : IViewModel, IDisposable
{
    /// <summary>
    /// Gets the unique identifier of the element currently being edited.
    /// </summary>
    string EditedElementIdentifier { get; }
}