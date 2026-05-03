namespace deavnote.app.EventArgs;

internal class ViewModelChangeEventArg : System.EventArgs
{
    public IViewModel ViewModel { get; private set; }

    public ViewModelChangeEventArg(IViewModel viewModel)
    {
        this.ViewModel = viewModel;
    }
}

