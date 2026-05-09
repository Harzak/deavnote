using System.Windows.Input;

namespace deavnote.app.Views;

internal sealed partial class MainView : Window
{
    public ICommand FocusSearchCommand { get; }


    public MainView()
    {
        this.FocusSearchCommand = new FocusSearchCommand(this);
        InitializeComponent();
    }
}