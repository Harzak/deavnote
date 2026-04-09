namespace deavnote.app.tests.Converters;

[TestClass]
public class TimeSpanToReadableDurationConverterTests
{
    [TestMethod]
    public void Convert_ShouldReturnReadableDuration_ForMaxTimeSpan()
    {
        // Arrange
        var converter = new TimeSpanToReadableDurationConverter();
        var timeSpan = new TimeSpan(1, 2, 30, 45); // 1 day, 2 hours, 30 minutes, 45 seconds

        // Act
        var result = converter.Convert(timeSpan, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual("1d 2h 30m 45s", result);
    }

    [TestMethod]
    public void Convert_ShouldReturnReadableDuration_ForMinTimeSpan()
    {
        // Arrange
        var converter = new TimeSpanToReadableDurationConverter();
        var timeSpan = new TimeSpan(0, 0, 0, 0); // 0 days, 0 hours, 0 minutes, 0 seconds

        // Act
        var result = converter.Convert(timeSpan, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void Convert_ShouldReturnReadableDuration_ForMediumTimeSpan()
    {
        // Arrange
        var converter = new TimeSpanToReadableDurationConverter();
        var timeSpan = new TimeSpan(0, 2, 30, 0); // 0 days, 0 hours, 0 minutes, 0 seconds

        // Act
        var result = converter.Convert(timeSpan, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual("2h 30m", result);
    }
}

