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
        JournalCursorsConfiguration configuration = new()
        {
            DateCursor = new DateTime(2026, 08, 10),
            TimeCursor = TimeSpan.FromDays(1)
        };

        //Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        A.CallTo(() => _repository.GetEntriesBetween(
                new DateTime(2026, 08, 10),
                new DateTime(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursors_ShouldPreLoadAdjacentEntries()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalCursorsConfiguration configuration = new()
        {
            DateCursor = new DateTime(2026, 08, 10),
            TimeCursor = TimeSpan.FromDays(1)
        };

        //Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        A.CallTo(() => _repository.GetEntriesBetween(
                new DateTime(2026, 08, 09),
                new DateTime(2026, 08, 10),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _repository.GetEntriesBetween(
                new DateTime(2026, 08, 11),
                new DateTime(2026, 08, 12),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursorsAsync_WhenIdenticalConfiguration_ShouldNotReloadEntries()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalCursorsConfiguration configuration = new()
        {
            DateCursor = new DateTime(2026, 08, 10),
            TimeCursor = TimeSpan.FromDays(1)
        };

        //Act
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.SetCursorsAsync(configuration, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        A.CallTo(() => _repository.GetEntriesBetween(
                new DateTime(2026, 08, 10),
                new DateTime(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetCursorsAsync_WhenAlreadyKnown_ShouldNotReloadEntries()
    {
        //Arrange
        Journal journal = new(_repository);
        JournalCursorsConfiguration configuration1 = new()
        {
            DateCursor = new DateTime(2026, 08, 10),
            TimeCursor = TimeSpan.FromDays(1)
        };
        JournalCursorsConfiguration configuration2 = new()
        {
            DateCursor = new DateTime(2026, 08, 15),
            TimeCursor = TimeSpan.FromDays(1)
        };

        //Act
        await journal.SetCursorsAsync(configuration1, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.SetCursorsAsync(configuration2, this.TestContext.CancellationToken).ConfigureAwait(false);
        await journal.SetCursorsAsync(configuration1, this.TestContext.CancellationToken).ConfigureAwait(false);

        //Assert
        A.CallTo(() => _repository.GetEntriesBetween(
                new DateTime(2026, 08, 10),
                new DateTime(2026, 08, 11),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
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
        journal.TimeCursor.Should().Be(journal.DefaultConfiguration.TimeCursor);
    }
}

