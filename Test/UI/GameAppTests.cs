using Xunit;

namespace Tests.UI
{
    public class GameAppTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var app = new Moutons.UI.GameApp(10, 8, 5, 3);

            // Assert
            Assert.NotNull(app);
        }
    }
}
