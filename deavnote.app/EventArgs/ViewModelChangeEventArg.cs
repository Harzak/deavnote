namespace deavnote.app.EventArgs;

internal class ViewModelChangeEventArg : System.EventArgs
{
    public IEditableViewModel ViewModel { get; private set; }

    public ViewModelChangeEventArg(IEditableViewModel viewModel)
    {
        this.ViewModel = viewModel;
    }
}

