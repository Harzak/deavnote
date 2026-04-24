namespace deavnote.app.tests.ViewModels;

[TestClass]
public class JournalViewModelTests
{
    private IJournal _journal;
    private IDateProvider _dateProvider;
    private IViewModelFactory _viewModelFactory;
    private IDialogService _dialogService;
    private INotificationService _notificationService;
    private IMessenger _messenger;
    private IClipboardService _clipboardService;

    [TestInitialize]
    public void Initialize()
    {
        _journal = A.Fake<IJournal>();
        _dateProvider = A.Fake<IDateProvider>();
        _viewModelFactory = A.Fake<IViewModelFactory>();
        _dialogService = A.Fake<IDialogService>();
        _notificationService = A.Fake<INotificationService>();
        _messenger = A.Fake<IMessenger>();
    }

    [TestMethod]
    public void Instanciation_ShouldInitialize()
    {
        // Arrange & Act
        var viewModel = new JournalViewModel(
            _journal,
            _dateProvider,
            _viewModelFactory,
            _dialogService,
            _notificationService,
            _messenger,
            _clipboardService);

        // Assert
        viewModel.TimeEntries.Should().NotBeNull();
        viewModel.IsLoading.Should().BeFalse();
        viewModel.HasErrors.Should().BeFalse();
    }
}

