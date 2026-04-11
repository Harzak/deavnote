namespace deavnote.core.tests.Services;

[TestClass]
public class ClipboardServiceTests
{

    private IClipboardInterop _clipboard;


    [TestInitialize]
    public void TestInitialize()
    {
        _clipboard = A.Fake<IClipboardInterop>();
    }

    [TestMethod]
    public async Task e()
    {
        // Arrange
        ClipboardService service = new(_clipboard);

        // Act
        //service.se

        // Assert
    }

}

