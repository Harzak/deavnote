namespace deavnote.app.Interfaces;

/// <summary>
/// Defines a service for displaying in-app toast notifications.
/// </summary>
internal interface INotificationService
{
    /// <summary>
    /// Gets the collection of active notifications, for binding in the UI.
    /// </summary>
    ObservableCollection<NotificationViewModel> Notifications { get; }

    /// <summary>
    /// Displays a notification.
    /// </summary>
    /// <param name="durationMs">Duration in milliseconds. Set to 0 to show indefinitely.</param>
    void Show(string message, ENotificationType type = ENotificationType.Info, int durationMs = 3000);
}
