namespace deavnote.core.tests.Services;

[TestClass]
public class JournalTests
{
    public TestContext TestContext { get; set; }

    private ITimeEntryRepository _repository;

    [TestInitialize]
    public void Initialize()
    {
        _repository = A.Fake<ITimeEntryRepository>();
    }

    [TestMethod]
    public async Task SetCursors_ShouldLoadEntries()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1
        };

        //Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 10),
                new DateOnly(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursors_ShouldPreLoadAdjacentEntries()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1
        };

        //Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 09),
                new DateOnly(2026, 08, 10),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 11),
                new DateOnly(2026, 08, 12),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursorsAsync_WhenIdenticalConfiguration_ShouldNotReloadEntries()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1
        };

        //Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 10),
                new DateOnly(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursorsAsync_WhenAlreadyKnown_ShouldNotReloadEntries()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalConfiguration configuration1 = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1
        };
        JournalConfiguration configuration2 = new()
        {
            DateCursor = new DateOnly(2026, 08, 15),
            DayOffset = 1
        };

        //Act
        await journal.SetCursorsAsync(configuration1, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.SetCursorsAsync(configuration2, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.SetCursorsAsync(configuration1, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 10),
                new DateOnly(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }


    [TestMethod]
    public async Task ShiftDate_WithSameDate_ShouldRetrieveResult()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalConfiguration configuration1 = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1
        };
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 10),
                new DateOnly(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
                new TimeEntry()
                {
                    Id = 1,
                    Code = "TEST",
                    Name = "Test Entry",
                    StartedAtUtc = new DateTime(2026, 08, 10, 8, 0, 0),
                }
            }.AsReadOnly());

        //Act
        await journal.SetCursorsAsync(configuration1, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.ShiftDateCursorAsync(-1, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.ShiftDateCursorAsync(1, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        journal.TimeEntries.Should().ContainSingle();
        journal.TimeEntries.ElementAt(0).Id.Should().Be(1);
        journal.TimeEntries.ElementAt(0).Code.Should().Be("TEST");
        journal.TimeEntries.ElementAt(0).Name.Should().Be("Test Entry");
    }

    [TestMethod]
    public async Task ShiftDate_WhenEmptyDays_ShouldNotRetrieveResult()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalConfiguration configuration1 = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1
        };
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 10),
                new DateOnly(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
                new TimeEntry()
                {
                    Id = 1,
                    Code = "TEST",
                    Name = "Test Entry",
                    StartedAtUtc = new DateTime(2026, 08, 10, 8, 0, 0),
                }
            }.AsReadOnly());

        //Act
        await journal.SetCursorsAsync(configuration1, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.ShiftDateCursorAsync(-1, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        journal.TimeEntries.Should().BeEmpty();
    }

    [TestMethod]
    public async Task LoadDefaultCursor_ShouldUseDefaultConfiguration()
    {
        //Arrange
        Journal journal = new(_repository);

        //Act
        await journal.LoadDefaultCursorAsync(this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        journal.DateCursor.Should().Be(journal.DefaultConfiguration.DateCursor);
        journal.DayOffset.Should().Be(journal.DefaultConfiguration.DayOffset);
    }
}

