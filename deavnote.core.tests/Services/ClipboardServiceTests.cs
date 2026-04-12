namespace deavnote.core.tests.Services;

[TestClass]
public class ClipboardServiceTests
{
    private IClipboardInterop _clipboard;
    private IClipboardFormatRepository _repository;

    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        _clipboard = A.Fake<IClipboardInterop>();
        _repository = A.Fake<IClipboardFormatRepository>();
    }

    [TestMethod]
    public async Task SetDailyTimeEntryAsync_ShouldInterpolateTemplate()
    {
        // Arrange
        JournalClipboardService service = new(_clipboard, _repository);
        A.CallTo(()
            => _repository.GetTemplateAsync(model.Enums.EJournalContext.DailySingle, A<CancellationToken>.Ignored))
            .Returns("entry name is: {EntryName} and work done is: {WorkDone}");

        // Act
        await service.SetDailyTimeEntryAsync(new TimeEntry
        {
            Name = "Refactor some stuff",
            WorkDone = "a lot of works"
        })
        .ConfigureAwait(false);

        // Assert
        string expected = "entry name is: Refactor some stuff and work done is: a lot of works";
        A.CallTo(() => _clipboard.SetTextAsync(expected))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetDailyTimeEntriesAsync_ShouldInterpolateTemplate()
    {
        // Arrange
        JournalClipboardService service = new(_clipboard, _repository);
        A.CallTo(()
            => _repository.GetTemplateAsync(model.Enums.EJournalContext.DailyMultiple, A<CancellationToken>.Ignored))
            .Returns("{EntryName}/{WorkDone}/{TaskName}/{TaskCode}");

        // Act
        await service.SetDailyTimeEntriesAsync([new TimeEntry
        {
            Name = "Entry1",
            WorkDone = "Work1",
            DevTask = new DevTask()
            {
                Name = "Task1",
                Code = "Code1"
            }
        },
        new TimeEntry
        {
            Name = "Entry2",
            WorkDone = "Work2",
            DevTask = new DevTask()
            {
                Name = "Task2",
                Code = "Code2"
            }
        }])
        .ConfigureAwait(false);

        // Assert
        string expected = "Entry1/Work1/Task1/Code1" + Environment.NewLine +
                          "Entry2/Work2/Task2/Code2" + Environment.NewLine;
        A.CallTo(() => _clipboard.SetTextAsync(expected))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetWeeklyTimeEntriesAsync_ShouldInterpolateTemplate()
    {
        // Arrange
        JournalClipboardService service = new(_clipboard, _repository);
        A.CallTo(()
            => _repository.GetTemplateAsync(model.Enums.EJournalContext.Weekly, A<CancellationToken>.Ignored))
            .Returns("{EntryName}/{WorkDone}/{TaskName}/{TaskCode}");

        // Act
        await service.SetWeeklyTimeEntriesAsync([new TimeEntry
        {
            Name = "Entry1",
            WorkDone = "Work1",
            DevTask = new DevTask()
            {
                Name = "Task1",
                Code = "Code1"
            }
        },
        new TimeEntry
        {
            Name = "Entry2",
            WorkDone = "Work2",
            DevTask = new DevTask()
            {
                Name = "Task2",
                Code = "Code2"
            }
        }])
        .ConfigureAwait(false);

        // Assert
        string expected = DateOnly.FromDateTime(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
                          "Entry1/Work1/Task1/Code1" + Environment.NewLine +
                          "Entry2/Work2/Task2/Code2" + Environment.NewLine;
        A.CallTo(() => _clipboard.SetTextAsync(expected))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetDailyTimeEntryAsync_WhenInvalidPlaceholder_ShouldFailedGracefully()
    {
        // Arrange
        JournalClipboardService service = new(_clipboard, _repository);
        A.CallTo(()
            => _repository.GetTemplateAsync(model.Enums.EJournalContext.DailySingle, A<CancellationToken>.Ignored))
            .Returns("entry name is: {InvalidPlaceHolder} and work done is: {WorkDone}");

        // Act
        await service.SetDailyTimeEntryAsync(new TimeEntry
        {
            Name = "Refactor some stuff",
            WorkDone = "a lot of works"
        })
        .ConfigureAwait(false);

        // Assert
        string expected = "entry name is: {InvalidPlaceHolder} and work done is: a lot of works";
        A.CallTo(() => _clipboard.SetTextAsync(expected))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetDailyTimeEntryAsync_WhenEmptyPlaceholder_ShouldFailedGracefully()
    {
        // Arrange
        JournalClipboardService service = new(_clipboard, _repository);
        A.CallTo(()
            => _repository.GetTemplateAsync(model.Enums.EJournalContext.DailySingle, A<CancellationToken>.Ignored))
            .Returns("task name: {TaskName}. task code: {TaskCode}. time entry name: {EntryName}. work done: {WorkDone}");

        // Act
        await service.SetDailyTimeEntryAsync(new TimeEntry
        {
            Name = string.Empty,
            WorkDone = null,
            DevTask = new DevTask()
            {
                Name = null!,
                Code = null!
            }
        })
        .ConfigureAwait(false);

        // Assert
        string expected = "task name: [Empty Task Name]. task code: [Empty Task Code]. time entry name: [Empty Entry Name]. work done: ";
        A.CallTo(() => _clipboard.SetTextAsync(expected))
            .MustHaveHappenedOnceExactly();
    }
}