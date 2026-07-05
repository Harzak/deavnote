namespace deavnote.core.tests.Services;

[TestClass]
public class ClipboardServiceTests
{
    private IClipboardInterop _clipboard;
    private IClipboardFormatRepository _repository;
    private ITemplateParser _templateParser;
    private ITemplateRenderer _templateRenderer;

    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        _clipboard = A.Fake<IClipboardInterop>();
        _repository = A.Fake<IClipboardFormatRepository>();
        _templateParser = A.Fake<ITemplateParser>();
        _templateRenderer = A.Fake<ITemplateRenderer>();
    }

    [TestMethod]
    public async Task SetTimeEntryAsync_CallsRendererAndSetsClipboard()
    {
        // Arrange
        JournalClipboardService service = new(_clipboard, _repository, _templateParser, _templateRenderer);
        const string templateString = "{EntryName}";
        const string renderedText = "Rendered text";
        TimeEntry entry = new() { Name = "Test entry" };

        A.CallTo(() => _repository.GetTemplateAsync(model.Enums.EJournalMode.TimeEntry, A<CancellationToken>.Ignored))
            .Returns(templateString);
        A.CallTo(() => _templateRenderer.RenderTimeEntry(templateString, entry))
            .Returns(renderedText);

        // Act
        await service.SetTimeEntryAsync(entry, TestContext.CancellationToken).ConfigureAwait(false);

        // Assert
        A.CallTo(() => _repository.GetTemplateAsync(model.Enums.EJournalMode.TimeEntry, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _templateRenderer.RenderTimeEntry(templateString, entry))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _clipboard.SetTextAsync(renderedText))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetDailyTimeEntriesAsync_ParsesTemplateAndCallsRenderer()
    {
        // Arrange
        JournalClipboardService service = new(_clipboard, _repository, _templateParser, _templateRenderer);
        const string templateString = "{{EACH_ENTRY}}\n{EntryName}\n{{END_EACH}}";
        TemplateSection parsedTemplate = new(string.Empty, "{EntryName}\n", string.Empty, hasLoop: true);
        const string renderedText = "Entry1\nEntry2\n";
        TimeEntry[] entries =
        [
            new TimeEntry { Name = "Entry1" },
            new TimeEntry { Name = "Entry2" },
        ];

        A.CallTo(() => _repository.GetTemplateAsync(model.Enums.EJournalMode.Day, A<CancellationToken>.Ignored))
            .Returns(templateString);
        A.CallTo(() => _templateParser.Parse(templateString))
            .Returns(parsedTemplate);
        A.CallTo(() => _templateRenderer.RenderTimeEntries(parsedTemplate, entries))
            .Returns(renderedText);

        // Act
        await service.SetDailyTimeEntriesAsync(entries, TestContext.CancellationToken).ConfigureAwait(false);

        // Assert
        A.CallTo(() => _repository.GetTemplateAsync(model.Enums.EJournalMode.Day, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _templateParser.Parse(templateString))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _templateRenderer.RenderTimeEntries(parsedTemplate, entries))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _clipboard.SetTextAsync(renderedText))
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task SetWeeklyTimeEntriesAsync_AddsDateHeaderAndCallsRenderer()
    {
        // Arrange
        JournalClipboardService service = new(_clipboard, _repository, _templateParser, _templateRenderer);
        const string templateString = "{EntryName}";
        TemplateSection parsedTemplate = new(string.Empty, "{EntryName}", string.Empty, hasLoop: false);
        const string renderedText = "Entry1";
        TimeEntry[] entries = [new TimeEntry { Name = "Entry1" }];

        A.CallTo(() => _repository.GetTemplateAsync(model.Enums.EJournalMode.Week, A<CancellationToken>.Ignored))
            .Returns(templateString);
        A.CallTo(() => _templateParser.Parse(templateString))
            .Returns(parsedTemplate);
        A.CallTo(() => _templateRenderer.RenderTimeEntries(parsedTemplate, entries))
            .Returns(renderedText);

        // Act
        await service.SetWeeklyTimeEntriesAsync(entries, TestContext.CancellationToken).ConfigureAwait(false);

        // Assert
        A.CallTo(() => _repository.GetTemplateAsync(model.Enums.EJournalMode.Week, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _templateParser.Parse(templateString))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _templateRenderer.RenderTimeEntries(parsedTemplate, entries))
            .MustHaveHappenedOnceExactly();

        // Verify clipboard text includes date header
        A.CallTo(() => _clipboard.SetTextAsync(A<string>.That.Matches(text =>
            text.StartsWith(DateOnly.FromDateTime(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture)))))
            .MustHaveHappenedOnceExactly();
    }
}