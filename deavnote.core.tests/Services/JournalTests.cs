namespace deavnote.core.tests.Services;

[TestClass]
public class JournalTests
{
    public TestContext TestContext { get; set; }
    private TimeProvider _timeProvider;

    private ITimeEntryRepository _repository;

    [TestInitialize]
    public void Initialize()
    {
        _repository = A.Fake<ITimeEntryRepository>();
        _timeProvider = A.Fake<TimeProvider>();
    }

    [TestMethod]
    public async Task SetCursors_ShouldLoadEntries()
    {
        //Arrange
        this.SetupTimeProviderForDateRange(+2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
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
        this.SetupTimeProviderForDateRange(+2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
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
        this.SetupTimeProviderForDateRange(+2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
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
        this.SetupTimeProviderForDateRange(+2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration1 = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
        };
        JournalConfiguration configuration2 = new()
        {
            DateCursor = new DateOnly(2026, 08, 15),
            DayOffset = 1,
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
    public async Task ShiftDate_WhenEmptyDays_ShouldNotRetrieveResult()
    {
        //Arrange
        this.SetupTimeProviderForDateRange(+2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration1 = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
        };
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 10),
                new DateOnly(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
                new()
                {
                    Id = 1,
                    Name = "Test Entry",
                    StartedAtUtc = new DateTime(2026, 08, 10, 8, 0, 0),
                },
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
        this.SetupTimeProviderForDateRange(+2);
        Journal journal = new(_repository, _timeProvider);

        //Act
        await journal.LoadDefaultCursorAsync(this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        journal.DateCursor.Should().Be(journal.DefaultConfiguration.DateCursor);
        journal.DayOffset.Should().Be(journal.DefaultConfiguration.DayOffset);
    }

    [TestMethod]
    public async Task Cursor_ShouldExcludeEntriesOutsideLocalRange_WhenNotUTC()
    {
        //Arrange
        this.SetupTimeProviderForDateRange(+2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
        };

        DateTime entryDateUtc = new DateTime(2026, 08, 09, 23, 30, 0, DateTimeKind.Utc);
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
            new DateOnly(2026, 08, 10),
            new DateOnly(2026, 08, 11),
            A<CancellationToken>.Ignored))
        .Returns(new List<TimeEntry>()
        {
            new()
            {
                Id = 1,
                Name = "Test Entry",
                StartedAtUtc = entryDateUtc,
            },
        }.AsReadOnly());

        //Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);
        TimeEntry? entry = journal.TimeEntries.FirstOrDefault(x => x.Id == 1);

        //Assert
        entry.Should().NotBeNull();
    }


    [TestMethod]
    public async Task Cursor_ShouldExcludeEntriesOutsideLocalRange_WhenUTC()
    {
        // Arrange
        this.SetupTimeProviderForDateRange(TimeZoneInfo.Utc);

        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
        };

        DateTime entryDateUtc = new DateTime(2026, 08, 09, 23, 30, 0, DateTimeKind.Utc);
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 08, 10),
                new DateOnly(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
                new() {
                    Id = 1,
                    Name = "Test Entry",
                    StartedAtUtc = entryDateUtc,
                },
            }.AsReadOnly());

        // Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);
        TimeEntry? entry = journal.TimeEntries.FirstOrDefault(x => x.Id == 1);

        // Assert
        entry.Should().BeNull();
    }

    [TestMethod]
    public async Task Cursor_ShouldExcludeEntriesOutsideLocalRange_WhenEntryIsOnPreviousLocalDay()
    {
        // Arrange
        this.SetupTimeProviderForDateRange(+2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 06, 21),
            DayOffset = 1,
        };

        DateTime entryDateUtc = new DateTime(2026, 06, 20, 21, 59, 0, DateTimeKind.Utc);
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                new DateOnly(2026, 06, 21),
                new DateOnly(2026, 06, 22),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
            new() { Id = 1, Name = "Test Entry", StartedAtUtc = entryDateUtc },
            }.AsReadOnly());

        // Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);
        TimeEntry? entry = journal.TimeEntries.FirstOrDefault(x => x.Id == 1);

        entry.Should().BeNull();
    }

    private void SetupTimeProviderForDateRange(int utcOffset)
    {
        TimeZoneInfo utcPlus2 = TimeZoneInfo.CreateCustomTimeZone($"UTC{utcOffset}", TimeSpan.FromHours(utcOffset), $"UTC+{utcOffset}", $"UTC{utcOffset}");
        this.SetupTimeProviderForDateRange(utcPlus2);
    }

    private void SetupTimeProviderForDateRange(TimeZoneInfo timeZoneInfo)
    {
        A.CallTo(() => _timeProvider.LocalTimeZone).Returns(timeZoneInfo);
    }
}

