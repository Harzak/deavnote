namespace deavnote.core.tests.Services.Template;

[TestClass]
public class TemplateRendererTests
{
    private TemplateRenderer _renderer;

    [TestInitialize]
    public void TestInitialize()
    {
        _renderer = new TemplateRenderer();
    }

    [TestMethod]
    public void Render_SimpleTemplate_ReplacesPlaceholders()
    {
        // Arrange
        const string template = "{TaskCode} - {TaskName}: {EntryName}";
        TimeEntry entry = new()
        {
            Name = "Fixed bug",
            DevTask = new DevTask()
            {
                Code = "DEV-123",
                Name = "Bug fix",
            },
        };

        // Act
        string result = _renderer.RenderTimeEntry(template, entry);

        // Assert
        result.Should().Be("DEV-123 - Bug fix: Fixed bug");
    }

    [TestMethod]
    public void Render_InvalidPlaceholder_LeavesPlaceholderUnchanged()
    {
        // Arrange
        const string template = "{TaskCode} - {InvalidPlaceholder}";
        TimeEntry entry = new()
        {
            Name = string.Empty,
            DevTask = new DevTask()
            {
                Name = string.Empty,
                Code = "DEV-456",
            },
        };

        // Act
        string result = _renderer.RenderTimeEntry(template, entry);

        // Assert
        result.Should().Be("DEV-456 - {InvalidPlaceholder}");
    }

    [TestMethod]
    public void Render_EmptyOrNullValues_UsesDefaultText()
    {
        // Arrange
        const string template = "{TaskName} | {TaskCode} | {EntryName} | {WorkDone}";
        TimeEntry entry = new()
        {
            Name = string.Empty,
            WorkDone = null,
            DevTask = new DevTask()
            {
                Name = null!,
                Code = null!,
            },
        };

        // Act
        string result = _renderer.RenderTimeEntry(template, entry);

        // Assert
        result.Should().Be("[Empty Task Name] | [Empty Task Code] | [Empty Entry Name] | ");
    }

    [TestMethod]
    public void RenderMultiple_SimpleTemplateWithoutLoop_RendersEachEntry()
    {
        // Arrange
        TemplateSection template = new(string.Empty, "{TaskCode}\n", string.Empty, hasLoop: false);
        TimeEntry[] entries =
        [
            new TimeEntry { Name = "Entry1", DevTask = new DevTask { Name = "Task1", Code = "DEV-001" } },
            new TimeEntry { Name = "Entry2", DevTask = new DevTask { Name = "Task2", Code = "DEV-002" } },
        ];

        // Act
        string result = _renderer.RenderTimeEntries(template, entries);

        // Assert
        result.Should().Be("DEV-001\nDEV-002\n");
    }

    [TestMethod]
    public void RenderMultiple_TemplateWithLoop_RendersHeaderBodyFooter()
    {
        // Arrange
        TemplateSection template = new("Hello,\n", "• {TaskCode}: {EntryName}\n", "Goodbye.", hasLoop: true);
        TimeEntry[] entries =
        [
            new TimeEntry { Name = "Work1", DevTask = new DevTask { Name = "Task1", Code = "DEV-100" } },
            new TimeEntry { Name = "Work2", DevTask = new DevTask { Name = "Task2", Code = "DEV-200" } },
        ];

        // Act
        string result = _renderer.RenderTimeEntries(template, entries);

        // Assert
        result.Should().Be("Hello,\n• DEV-100: Work1\n• DEV-200: Work2\nGoodbye.");
    }

    [TestMethod]
    public void RenderMultiple_EmptyHeader_OnlyRendersBodyAndFooter()
    {
        // Arrange
        TemplateSection template = new(string.Empty, "{TaskCode}\n", "End", hasLoop: true);
        TimeEntry[] entries =
        [
            new TimeEntry { Name = "Entry", DevTask = new DevTask { Name = "Task", Code = "ABC" } },
        ];

        // Act
        string result = _renderer.RenderTimeEntries(template, entries);

        // Assert
        result.Should().Be("ABC\nEnd");
    }

    [TestMethod]
    public void RenderMultiple_EmptyFooter_OnlyRendersHeaderAndBody()
    {
        // Arrange
        TemplateSection template = new("Start\n", "{TaskCode}\n", string.Empty, hasLoop: true);
        TimeEntry[] entries =
        [
            new TimeEntry { Name = "Entry", DevTask = new DevTask { Name = "Task", Code = "XYZ" } },
        ];

        // Act
        string result = _renderer.RenderTimeEntries(template, entries);

        // Assert
        result.Should().Be("Start\nXYZ\n");
    }
}