namespace deavnote.core.tests.Services;

[TestClass]
public class JournalTests
{
    private ITimeEntryRepository _repository;

    [TestInitialize]
    public void Initialize()
    {
        _repository = A.Fake<ITimeEntryRepository>();
    }

    [TestMethod]
    public async Task TestJournal()
    {
        //Arrange
        Journal journal = new (_repository);
        JournalCursorsConfiguration configuration = new JournalCursorsConfiguration
        {
            DateCursor = DateTime.Today,
            TimeCursor = TimeSpan.FromDays(1)
        };

        //Act
        await journal.LoadDefaultCursorAsync().ConfigureAwait(false);

        //Assert
        journal.DateCursor.Should().Be(journal.DefaultConfiguration.DateCursor);
        journal.TimeCursor.Should().Be(journal.DefaultConfiguration.TimeCursor);
    }
}

