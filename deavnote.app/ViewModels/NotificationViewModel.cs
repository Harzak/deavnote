namespace deavnote.app.ViewModels;

internal sealed partial class NotificationViewModel : BaseViewModel
{
    private readonly Action<NotificationViewModel> _onClose;

    public override string Identifier { get; }
    public ENotificationType Type { get; }
    public string Message { get; }

    public NotificationViewModel(ENotificationType type, string message, Action<NotificationViewModel> onClose)
    {
        ArgumentNullException.ThrowIfNull(onClose);

        this.Type = type;
        this.Message = message;
        this._onClose = onClose;
        this.Identifier = Guid.NewGuid().ToString();
    }

    [RelayCommand]
    private void Close()
    {
        _onClose(this);
    }
}

