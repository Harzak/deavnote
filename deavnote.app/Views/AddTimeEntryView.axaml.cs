using Avalonia.Interactivity;

namespace deavnote.app.Views;

internal sealed partial class AddTimeEntryView : UserControl
{
    public AddTimeEntryView()
    {
        InitializeComponent();
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (this.DataContext is AddTimeEntryViewModel viewModel)
        {
            await viewModel.InitializedAsync().ConfigureAwait(false);
        }
    }
}
