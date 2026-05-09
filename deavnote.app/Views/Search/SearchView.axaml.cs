namespace deavnote.app.Views.Search;

internal sealed partial class SearchView : UserControl
{
    public SearchView()
    {
        InitializeComponent();
    }

    internal void FocusSearch()
    {
        this.SearchBox.FocusSearch();
    }
}