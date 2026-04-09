namespace deavnote.app.tests.ViewModels;

[TestClass]
public class JournalViewModelTests
{
    private IJournal _journal;
    private IDateProvider _dateProvider;
    private IViewModelFactory _viewModelFactory;
    private IDialogService _dialogService;

    [TestInitialize]
    public void Initialize()
    {
        _journal = A.Fake<IJournal>();
        _dateProvider = A.Fake<IDateProvider>();
        _viewModelFactory = A.Fake<IViewModelFactory>();
        _dialogService = A.Fake<IDialogService>();
    }

    [TestMethod]
    public void Instanciation_ShouldInitialize()
    {
        // Arrange & Act
        var viewModel = new JournalViewModel(_journal, _dateProvider, _viewModelFactory, _dialogService);

        // Assert
        viewModel.TimeEntries.Should().NotBeNull();   
        viewModel.IsLoading.Should().BeFalse();
        viewModel.HasErrors.Should().BeFalse();
    }
}

