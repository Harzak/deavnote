namespace deavnote.core.tests.Services.Template;

[TestClass]
public class TemplateParserTests
{
    private TemplateParser _parser;

    [TestInitialize]
    public void TestInitialize()
    {
        _parser = new TemplateParser();
    }

    [TestMethod]
    public void Parse_SimpleTemplateWithoutDelimiters_ReturnsSimpleSection()
    {
        // Arrange
        const string template = "Task: {TaskCode} - {TaskName}";

        // Act
        TemplateSection result = _parser.Parse(template);

        // Assert
        result.Should().NotBeNull();
        result.HasLoop.Should().BeFalse();
        result.Header.Should().BeEmpty();
        result.Body.Should().Be("Task: {TaskCode} - {TaskName}");
        result.Footer.Should().BeEmpty();
    }

    [TestMethod]
    public void Parse_TemplateWithLoopDelimiters_ExtractsHeaderBodyFooter()
    {
        // Arrange
        const string template = "Hello,\n{{EACH_ENTRY}}\n• {TaskCode}: {EntryName}\n{{END_EACH}}\nAurélien.";

        // Act
        TemplateSection result = _parser.Parse(template);

        // Assert
        result.Should().NotBeNull();
        result.HasLoop.Should().BeTrue();
        result.Header.Should().Be("Hello,\n");
        result.Body.Should().Be("• {TaskCode}: {EntryName}\n");
        result.Footer.Should().Be("Aurélien.");
    }

    [TestMethod]
    public void Parse_TemplateWithOnlyStartDelimiter_ReturnsSimpleSection()
    {
        // Arrange
        const string template = "{{EACH_ENTRY}}\n{TaskCode}";

        // Act
        TemplateSection result = _parser.Parse(template);

        // Assert
        result.Should().NotBeNull();
        result.HasLoop.Should().BeFalse();
        result.Header.Should().BeEmpty();
        result.Body.Should().Be("{{EACH_ENTRY}}\n{TaskCode}");
    }

    [TestMethod]
    public void Parse_TemplateWithOnlyEndDelimiter_ReturnsSimpleSection()
    {
        // Arrange
        const string template = "{TaskCode}\n{{END_EACH}}";

        // Act
        TemplateSection result = _parser.Parse(template);

        // Assert
        result.Should().NotBeNull();
        result.HasLoop.Should().BeFalse();
        result.Header.Should().BeEmpty();
        result.Body.Should().Be("{TaskCode}\n{{END_EACH}}");
        result.Footer.Should().BeEmpty();
    }

    [TestMethod]
    public void Parse_EndDelimiterBeforeStartDelimiter_ThrowsInvalidOperationException()
    {
        // Arrange
        const string template = "{{END_EACH}}\n{TaskCode}\n{{EACH_ENTRY}}";

        // Act
        Action act = () => _parser.Parse(template);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*{{END_EACH}} appears before {{EACH_ENTRY}}*");
    }

    [TestMethod]
    public void Parse_DelimiterLinesAreCompletelyRemoved()
    {
        // Arrange
        const string template = "Start\n{{EACH_ENTRY}}\nBody\n{{END_EACH}}\nEnd";

        // Act
        TemplateSection result = _parser.Parse(template);

        // Assert
        result.Header.Should().Be("Start\n");
        result.Body.Should().Be("Body\n");
        result.Footer.Should().Be("End");
        result.Header.Should().NotContain("{{EACH_ENTRY}}");
        result.Body.Should().NotContain("{{EACH_ENTRY}}");
        result.Body.Should().NotContain("{{END_EACH}}");
        result.Footer.Should().NotContain("{{END_EACH}}");
    }

    [TestMethod]
    public void Parse_EmptyHeaderAndFooter_WorksCorrectly()
    {
        // Arrange
        const string template = "{{EACH_ENTRY}}\n{TaskCode}\n{{END_EACH}}";

        // Act
        TemplateSection result = _parser.Parse(template);

        // Assert
        result.Should().NotBeNull();
        result.HasLoop.Should().BeTrue();
        result.Header.Should().BeEmpty();
        result.Body.Should().Be("{TaskCode}\n");
        result.Footer.Should().BeEmpty();
    }
}
