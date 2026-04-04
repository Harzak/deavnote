using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace deavnote.app.Views;

internal sealed partial class JournalView : UserControl
{
    public JournalView()
    {
        InitializeComponent();
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (this.DataContext is JournalViewModel viewModel)
        {
            await viewModel.InitializedAsync().ConfigureAwait(false);
        }
    }
}