using deavnote.app.ViewModels.DevTask;
using deavnote.repository.Dto;
using deavnote.utils.Results;

namespace deavnote.app.tests.ViewModels.DevTask;

[TestClass]
public class DevTaskDetailViewModelTests
{
    private INotificationService _notificationService;
    private IDevTaskRepository _repository;

    [TestInitialize]
    public void Initialize()
    {
        _notificationService = A.Fake<INotificationService>();
        _repository = A.Fake<IDevTaskRepository>();
    }

    [TestMethod]
    public async Task Properties_WhenRequired_ShouldPreventSaving()
    {
        // Arrange
        model.Entities.DevTask model = new()
        {
            Id = 1,
            Name = "Initial Task",
            Code = "Test",
            Description = "Initial description.",
        };
        using DevTaskDetailViewModel viewModel = new(model, isReadonly: false, _repository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.Name = "";

        // Assert
        viewModel.HasErrors.Should().BeTrue();
        viewModel.GetErrors(nameof(viewModel.Name)).Should().ContainSingle();
        viewModel.HasChanges.Should().BeTrue();
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeTrue();
    }

    [TestMethod]
    public async Task ModifyingProperty_WhenTracked_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.DevTask model = new()
        {
            Id = 1,
            Name = "Initial Task",
            Code = "Test",
            Description = "Initial description.",
        };
        using DevTaskDetailViewModel viewModel = new(model, isReadonly: false, _repository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.Description += " Additional description.";

        // Assert
        viewModel.HasChanges.Should().BeTrue();
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeTrue();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeTrue();
    }

    [TestMethod]
    public async Task Save_WithChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.DevTask model = new()
        {
            Id = 1,
            Name = "Initial Task",
            Code = "Test",
            Description = "Initial description.",
        };
        A.CallTo(() => _repository.UpdateTaskAsync(A<UpdateDevTaskRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(OperationResult.Success()));
        using DevTaskDetailViewModel viewModel = new(model, isReadonly: false, _repository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.Description += " Additional description.";
        await viewModel.SaveCommand.ExecuteAsync(parameter: null).ConfigureAwait(false);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
        A.CallTo(() => _repository.UpdateTaskAsync(new UpdateDevTaskRequest()
        {
            Id = model.Id,
            Name = viewModel.Name,
            Code = viewModel.Code,
            Description = viewModel.Description,
        }, A<CancellationToken>._))
        .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task Cancel_WithChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.DevTask model = new()
        {
            Id = 1,
            Name = "Initial Task",
            Code = "Test",
            Description = "Initial description.",
        };
        using DevTaskDetailViewModel viewModel = new(model, isReadonly: false, _repository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.Description += " Additional description.";
        viewModel.CancelCommand.Execute(parameter: null);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.Description.Should().Be(model.Description);
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
    }

    [TestMethod]
    public async Task UndoChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.DevTask model = new()
        {
            Id = 1,
            Name = "Initial Task",
            Code = "Test",
            Description = "Initial description.",
        };
        using DevTaskDetailViewModel viewModel = new(model, isReadonly: false, _repository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);
        const string text = " Additional description.";

        // Act
        viewModel.Description += text;
        viewModel.Description = viewModel.Description.Replace(text, "", StringComparison.OrdinalIgnoreCase);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.Description.Should().Be(model.Description);
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
    }
}