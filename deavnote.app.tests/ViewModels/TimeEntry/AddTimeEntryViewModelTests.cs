using deavnote.app.ViewModels.TimeEntry;

namespace deavnote.app.tests.ViewModels.TimeEntry;

[TestClass]
public class AddTimeEntryViewModelTests
{
    private IDevTaskRepository _taskRepository;
    private ILocalizationService _localizationService;

    [TestInitialize]
    public void Initialize()
    {
        _taskRepository = A.Fake<IDevTaskRepository>();
        _localizationService = A.Fake<ILocalizationService>();
    }

    [TestMethod]
    public async Task Properties_WhenRequired_ShouldPreventSaving()
    {
        // Arrange
        A.CallTo(() => _localizationService.GetString(A<string>._))
            .ReturnsLazily((string key) => key);
        AddTimeEntryViewModel viewModel = new(_taskRepository, _localizationService);
        await viewModel.InitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.EntryTaskLink =  Enums.ETimeEntryCreationTaskLink.CreateNewTask;
        viewModel.EntryName = "input";
        viewModel.EntryName = "";
        viewModel.EntryDuration = TimeSpan.MinValue;
        viewModel.SearchTaskCode = "input";
        viewModel.SearchTaskCode = "";
        viewModel.SearchTaskName= "input";
        viewModel.SearchTaskName= "";

        // Assert
        viewModel.HasErrors.Should().BeTrue();
        viewModel.GetErrors(nameof(viewModel.EntryName)).Should().ContainSingle();
        viewModel.GetErrors(nameof(viewModel.EntryDuration)).Should().ContainSingle();
        viewModel.GetErrors(nameof(viewModel.SearchTaskCode)).Should().ContainSingle();
        viewModel.GetErrors(nameof(viewModel.SearchTaskName)).Should().ContainSingle();
        viewModel.ConfirmCommand.CanExecute(parameter: null).Should().BeFalse();
    }
}
