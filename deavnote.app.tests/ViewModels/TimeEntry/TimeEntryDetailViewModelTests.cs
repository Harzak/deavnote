using deavnote.app.ViewModels.TimeEntry;
using deavnote.repository.Dto;
using deavnote.utils.Results;

namespace deavnote.app.tests.ViewModels.TimeEntry;

[TestClass]
public class TimeEntryDetailViewModelTests
{
    private IJournal _journal;
    private IDevTaskRepository _taskRepository;
    private INotificationService _notificationService;

    [TestInitialize]
    public void Initialize()
    {
        _journal = A.Fake<IJournal>();
        _taskRepository = A.Fake<IDevTaskRepository>();
        _notificationService = A.Fake<INotificationService>();
    }

    [TestMethod]
    public async Task Properties_WhenRequired_ShouldPreventSaving()
    {
        // Arrange
        model.Entities.DevTask task = new()
        {
            Id = 1,
            Name = "Test Task",
            Code = "TT-001",
            Description = "Task description",
            State = deavnote.model.Enums.EDevTaskState.InProgress,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            TaskId = task.Id,
            Name = "Test Entry",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
        };
        A.CallTo(() => _taskRepository.GetTaskAsync(task.Id, A<CancellationToken>._))
            .Returns(Task.FromResult<model.Entities.DevTask?>(task));
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _taskRepository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.EntryName = "";

        // Assert
        viewModel.HasErrors.Should().BeTrue();
        viewModel.GetErrors(nameof(viewModel.EntryName)).Should().ContainSingle();
        viewModel.HasChanges.Should().BeTrue();
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeTrue();
    }

    [TestMethod]
    public async Task ModifyingProperty_WhenTracked_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.DevTask task = new()
        {
            Id = 1,
            Name = "Test Task",
            Code = "TT-001",
            Description = "Task description",
            State = deavnote.model.Enums.EDevTaskState.InProgress,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            TaskId = task.Id,
            Name = "Test Entry",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
        };
        A.CallTo(() => _taskRepository.GetTaskAsync(task.Id, A<CancellationToken>._))
            .Returns(Task.FromResult<model.Entities.DevTask?>(task));
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _taskRepository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.TaskDescription += " Additional work done.";

        // Assert
        viewModel.HasChanges.Should().BeTrue();
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeTrue();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeTrue();
    }

    [TestMethod]
    public async Task Save_WithChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.DevTask task = new()
        {
            Id = 1,
            Name = "Test Task",
            Code = "TT-001",
            Description = "Task description",
            State = deavnote.model.Enums.EDevTaskState.InProgress,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            TaskId = task.Id,
            Name = "Test Entry",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
        };
        A.CallTo(() => _taskRepository.GetTaskAsync(task.Id, A<CancellationToken>._))
            .Returns(Task.FromResult<model.Entities.DevTask?>(task));
        A.CallTo(() => _taskRepository.UpdateTaskAsync(A<UpdateDevTaskRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(OperationResult.Success()));
        A.CallTo(() => _journal.UpdateEntryAsync(A<UpdateTimeEntryRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(OperationResult.Success()));
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _taskRepository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.TaskDescription += " Additional work done.";
        await viewModel.SaveCommand.ExecuteAsync(parameter: null).ConfigureAwait(false);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
        A.CallTo(() => _taskRepository.UpdateTaskAsync(A<UpdateDevTaskRequest>.That.Matches(req =>
            req.Id == task.Id &&
            req.Name == viewModel.TaskName &&
            req.Code == viewModel.TaskCode &&
            req.Description == viewModel.TaskDescription &&
            req.State == viewModel.TaskState), A<CancellationToken>._))
        .MustHaveHappenedOnceExactly();
        A.CallTo(() => _journal.UpdateEntryAsync(A<UpdateTimeEntryRequest>.That.Matches(req =>
            req.Id == model.Id &&
            req.Name == viewModel.EntryName &&
            req.StartedAt == viewModel.EntryStartedAt.DateTime &&
            req.Duration == viewModel.EntryDuration), A<CancellationToken>._))
        .MustHaveHappenedOnceExactly();
    }


    [TestMethod]
    public async Task Cancel_WithChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.DevTask task = new()
        {
            Id = 1,
            Name = "Test Task",
            Code = "TT-001",
            Description = "Task description",
            State = deavnote.model.Enums.EDevTaskState.InProgress,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            TaskId = task.Id,
            Name = "Test Entry",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
        };
        A.CallTo(() => _taskRepository.GetTaskAsync(task.Id, A<CancellationToken>._))
            .Returns(Task.FromResult<model.Entities.DevTask?>(task));
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _taskRepository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        viewModel.TaskDescription += " Additional work done.";
        viewModel.CancelCommand.Execute(parameter: null);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.TaskDescription.Should().Be(task.Description);
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
    }

    [TestMethod]
    public async Task UndoChanges_ShouldSetDirtyState()
    {
        // Arrange
        model.Entities.DevTask task = new()
        {
            Id = 1,
            Name = "Test Task",
            Code = "TT-001",
            Description = "Task description",
            State = deavnote.model.Enums.EDevTaskState.InProgress,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        model.Entities.TimeEntry model = new()
        {
            Id = 1,
            TaskId = task.Id,
            Name = "Test Entry",
            StartedAtUtc = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(2),
        };
        A.CallTo(() => _taskRepository.GetTaskAsync(task.Id, A<CancellationToken>._))
            .Returns(Task.FromResult<model.Entities.DevTask?>(task));
        using TimeEntryDetailViewModel viewModel = new(model, _journal, _taskRepository, _notificationService);
        await viewModel.OnInitializedAsync().ConfigureAwait(false);

        // Act
        const string text = " Additional work done.";
        viewModel.TaskDescription += text;
        viewModel.TaskDescription = viewModel.TaskDescription.Replace(text, "", StringComparison.OrdinalIgnoreCase);

        // Assert
        viewModel.HasChanges.Should().BeFalse();
        viewModel.TaskDescription.Should().Be(task.Description);
        viewModel.SaveCommand.CanExecute(parameter: null).Should().BeFalse();
        viewModel.CancelCommand.CanExecute(parameter: null).Should().BeFalse();
    }
}