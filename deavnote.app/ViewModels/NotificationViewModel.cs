namespace deavnote.app.ViewModels;

internal sealed partial class NotificationViewModel : BaseViewModel
{
    private readonly Action<NotificationViewModel> _onClose;

    public ENotificationType Type { get; }
    public string Message { get; }

    public NotificationViewModel(ENotificationType type, string message, Action<NotificationViewModel> onClose)
    {
        ArgumentNullException.ThrowIfNull(onClose);

        this.Type = type;
        this.Message = message;
        this._onClose = onClose;
    }

    [RelayCommand]
    private void Close()
    {
        _onClose(this);
    }
}

