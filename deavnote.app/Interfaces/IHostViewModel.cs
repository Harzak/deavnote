namespace deavnote.app.Interfaces;

/// <summary>
/// Represents the view model for a host, providing state information about its current activity.
/// </summary>
internal interface IHostViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the system is currently performing a task.
    /// </summary>
    bool IsBusy { get; set; }
}