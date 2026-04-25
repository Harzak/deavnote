namespace deavnote.app.tests.Converters;

[TestClass]
public class EnumDisplayNameConverterTests
{
    [TestMethod]
    public void Convert_ShouldReturnDisplayName_WhenDisplayAttributeIsPresent()
    {
        // Arrange
        EnumDisplayNameConverter converter = new();
        EDevTaskState value = EDevTaskState.InProgress;

        // Act
        object? result = converter.Convert(value, typeof(string), parameter: null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be("In progress");
    }
}
