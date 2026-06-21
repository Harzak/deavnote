namespace deavnote.core.tests.Services;

[TestClass]
public class JournalTests
{
    public TestContext TestContext { get; set; }
    private TimeProvider _timeProvider;
    private ITimeEntryRepository _repository;

    private static readonly TimeZoneInfo UtcPlus2 = TimeZoneInfo.CreateCustomTimeZone("UTC+2", TimeSpan.FromHours(2), "UTC+2", "UTC+2");

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
        this.SetupTimeProvider(UtcPlus2);
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
                LocalMidnightAsUtc(new DateOnly(2026, 08, 10), UtcPlus2),
                LocalMidnightAsUtc(new DateOnly(2026, 08, 11), UtcPlus2),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursors_ShouldPreLoadAdjacentEntries()
    {
        //Arrange
        this.SetupTimeProvider(UtcPlus2);
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
                LocalMidnightAsUtc(new DateOnly(2026, 08, 09), UtcPlus2),
                LocalMidnightAsUtc(new DateOnly(2026, 08, 10), UtcPlus2),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                LocalMidnightAsUtc(new DateOnly(2026, 08, 11), UtcPlus2),
                LocalMidnightAsUtc(new DateOnly(2026, 08, 12), UtcPlus2),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursorsAsync_WhenIdenticalConfiguration_ShouldNotReloadEntries()
    {
        //Arrange
        this.SetupTimeProvider(UtcPlus2);
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
                LocalMidnightAsUtc(new DateOnly(2026, 08, 10), UtcPlus2),
                LocalMidnightAsUtc(new DateOnly(2026, 08, 11), UtcPlus2),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursorsAsync_WhenAlreadyKnown_ShouldNotReloadEntries()
    {
        //Arrange
        this.SetupTimeProvider(UtcPlus2);
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
                LocalMidnightAsUtc(new DateOnly(2026, 08, 10), UtcPlus2),
                LocalMidnightAsUtc(new DateOnly(2026, 08, 11), UtcPlus2),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task ShiftDate_WhenEmptyDays_ShouldNotRetrieveResult()
    {
        //Arrange
        this.SetupTimeProvider(UtcPlus2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
        };
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                LocalMidnightAsUtc(new DateOnly(2026, 08, 10), UtcPlus2),
                LocalMidnightAsUtc(new DateOnly(2026, 08, 11), UtcPlus2),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
                new()
                {
                    Id = 1,
                    Name = "Test Entry",
                    StartedAtUtc = new DateTime(2026, 08, 10, 8, 0, 0, DateTimeKind.Utc),
                },
            }.AsReadOnly());

        //Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.ShiftDateCursorAsync(-1, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        journal.TimeEntries.Should().BeEmpty();
    }

    [TestMethod]
    public async Task LoadDefaultCursor_ShouldUseDefaultConfiguration()
    {
        //Arrange
        this.SetupTimeProvider(UtcPlus2);
        Journal journal = new(_repository, _timeProvider);

        //Act
        await journal.LoadDefaultCursorAsync(this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        journal.DateCursor.Should().Be(journal.DefaultConfiguration.DateCursor);
        journal.DayOffset.Should().Be(journal.DefaultConfiguration.DayOffset);
    }

    [TestMethod]
    public async Task Cursor_ShouldIncludeEntryAtLocalMidnight_WhenNotUTC()
    {
        // Arrange
        // 2026-08-09 22:00 UTC = 2026-08-10 00:00 UTC+2 → first moment of the cursor day → must be included
        this.SetupTimeProvider(UtcPlus2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
        };

        DateTime entryStartUtc = new DateTime(2026, 08, 09, 22, 0, 0, DateTimeKind.Utc);
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                LocalMidnightAsUtc(new DateOnly(2026, 08, 10), UtcPlus2),
                LocalMidnightAsUtc(new DateOnly(2026, 08, 11), UtcPlus2),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
                new() { Id = 1, Name = "Test Entry", StartedAtUtc = entryStartUtc },
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
        // 2026-08-09 23:30 UTC with UTC timezone → local date is Aug 9 → not in Aug 10 cursor
        this.SetupTimeProvider(TimeZoneInfo.Utc);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 08, 10),
            DayOffset = 1,
        };

        DateTime entryDateUtc = new DateTime(2026, 08, 09, 23, 30, 0, DateTimeKind.Utc);
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                LocalMidnightAsUtc(new DateOnly(2026, 08, 10), TimeZoneInfo.Utc),
                LocalMidnightAsUtc(new DateOnly(2026, 08, 11), TimeZoneInfo.Utc),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
                new() { Id = 1, Name = "Test Entry", StartedAtUtc = entryDateUtc },
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
        // 2026-06-20 21:59 UTC = 2026-06-20 23:59 UTC+2 → local date is Jun 20 → not in Jun 21 cursor
        this.SetupTimeProvider(UtcPlus2);
        Journal journal = new(_repository, _timeProvider);
        JournalConfiguration configuration = new()
        {
            DateCursor = new DateOnly(2026, 06, 21),
            DayOffset = 1,
        };

        DateTime entryDateUtc = new DateTime(2026, 06, 20, 21, 59, 0, DateTimeKind.Utc);
        A.CallTo(() => _repository.GetEntriesBetweenAsync(
                LocalMidnightAsUtc(new DateOnly(2026, 06, 21), UtcPlus2),
                LocalMidnightAsUtc(new DateOnly(2026, 06, 22), UtcPlus2),
                A<CancellationToken>.Ignored))
            .Returns(new List<TimeEntry>()
            {
                new() { Id = 1, Name = "Test Entry", StartedAtUtc = entryDateUtc },
            }.AsReadOnly());

        // Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);
        TimeEntry? entry = journal.TimeEntries.FirstOrDefault(x => x.Id == 1);

        // Assert
        entry.Should().BeNull();
    }

    private static DateTime LocalMidnightAsUtc(DateOnly localDate, TimeZoneInfo zone) =>
        TimeZoneInfo.ConvertTimeToUtc(localDate.ToDateTime(TimeOnly.MinValue), zone);

    private void SetupTimeProvider(TimeZoneInfo timeZoneInfo)
    {
        A.CallTo(() => _timeProvider.LocalTimeZone).Returns(timeZoneInfo);
        A.CallTo(() => _timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 06, 21, 12, 0, 0, TimeSpan.Zero));
    }
}

