namespace deavnote.app.Services;

internal sealed class NotificationService : INotificationService
{
    /// <inheritdoc />
    public ObservableCollection<NotificationViewModel> Notifications { get; }

    public NotificationService()
    {
        this.Notifications = [];
    }

    /// <inheritdoc />
    public void Show(string message, ENotificationType type = ENotificationType.Info, int durationMs = 3000)
    {
        NotificationViewModel notification = new(type, message, Remove);
        Dispatcher.UIThread.Post(() =>
        {
            Notifications.Add(notification);
        });

        if (durationMs > 0)
        {
            _ = Task.Delay(durationMs)
                .ContinueWith(_ =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        Notifications.Remove(notification);
                    });
                }, TaskScheduler.Default);
        }
    }

    private void Remove(NotificationViewModel notification)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Notifications.Remove(notification);
        });
    }
}
