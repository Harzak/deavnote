using deavnote.app.ViewModels.TimeEntry;
using deavnote.repository.Dto;
using deavnote.utils.Results;

namespace deavnote.app.tests.ViewModels.TimeEntry;

[TestClass]
public class TimeEntryDetailViewModelTests
{
    private IJournal _journal;
    private IViewModelFactory _factory;
    private INotificationService _notificationService;

    [TestInitialize]
    public void Initialize()
    {
        _journal = A.Fake<IJournal>();
        _factory = A.Fake<IViewModelFactory>();
        _notificationService = A.Fake<INotificationService>();
    }

    [TestMethod]
    public async Task Properties_WhenRequired_ShouldPreventSaving()
    {
        // Arrange
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            Name = "Test Entry",
            WorkDone = "Worked on testing.",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
            DevTask = new model.Entities.DevTask
            {
                Id = 1,
                Name = "Test Task",
                Code = "TT-001",
            },
        };
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _factory, _notificationService);
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
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            Name = "Test Entry",
            WorkDone = "Worked on testing.",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
            DevTask = new model.Entities.DevTask
            {
                Id = 1,
                Name = "Test Task",
                Code = "TT-001",
            },
        };
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _factory, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.WorkDone += " Additional work done.";

        // Assert
        viewModel.HasChanges.Should().BeTrue();
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeTrue();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeTrue();
    }

    [TestMethod]
    public async Task Save_WithChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            Name = "Test Entry",
            WorkDone = "Worked on testing.",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
            DevTask = new model.Entities.DevTask
            {
                Id = 1,
                Name = "Test Task",
                Code = "TT-001",
            },
        };
        A.CallTo(() => _journal.UpdateEntryAsync(A<UpdateTimeEntryRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(OperationResult.Success()));
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _factory, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.WorkDone += " Additional work done.";
        await viewModel.SaveCommand.ExecuteAsync(parameter: null).ConfigureAwait(false);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
        A.CallTo(() => _journal.UpdateEntryAsync(new UpdateTimeEntryRequest()
        {
            Id = model.Id,
            Name = viewModel.Name,
            WorkDone = viewModel.WorkDone,
            StartedAt = viewModel.StartedAt.UtcDateTime,
            Duration = viewModel.Duration,
        }, A<CancellationToken>._))
        .MustHaveHappenedOnceExactly();
    }


    [TestMethod]
    public async Task Cancel_WithChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            Name = "Test Entry",
            WorkDone = "Worked on testing.",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
            DevTask = new model.Entities.DevTask
            {
                Id = 1,
                Name = "Test Task",
                Code = "TT-001",
            },
        };
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _factory, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.WorkDone += " Additional work done.";
        viewModel.CancelCommand.Execute(parameter: null);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.WorkDone.Should().Be(model.WorkDone);
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
    }

    [TestMethod]
    public async Task UndoChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            Name = "Test Entry",
            WorkDone = "Worked on testing.",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
            DevTask = new model.Entities.DevTask
            {
                Id = 1,
                Name = "Test Task",
                Code = "TT-001",
            },
        };
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _factory, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        const string text = " Additional work done.";
        viewModel.WorkDone += text;
        viewModel.WorkDone = viewModel.WorkDone.Replace(text, "", StringComparison.OrdinalIgnoreCase);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.WorkDone.Should().Be(model.WorkDone);
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
    }
}