namespace deavnote.app.ViewModels;

internal sealed partial class ConfirmationViewModel : DialogViewModel<EConfirmationResult>
{
    internal override string Title => "Confirm";

    [ObservableProperty]
    public partial string Message { get; set; }

    public ConfirmationViewModel(string message)
    {
        this.Message = message;
    }

    [RelayCommand]
    private void Confirm()
    {
        base.Close(EConfirmationResult.Yes);
    }


    [RelayCommand]
    private void Decline()
    {
        base.Close(EConfirmationResult.No);
    }

    [RelayCommand]
    private void Cancel()
    {
        base.Close(EConfirmationResult.Cancel);
    }
}