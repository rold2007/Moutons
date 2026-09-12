using System.Drawing;
using Moutons.Core;
using Xunit;

namespace Tests.Core;

public class EntityManagerTests
{
    [Fact]
    public void Constructor_WithZeroPoint_SetsSheepPosition()
    {
        // Arrange
        Point p = new Point(0, 0);

        // Act
        EntityManager manager = new EntityManager(p);

        // Assert
        Assert.Equal(p, manager.SheepPosition);
    }

    [Fact]
    public void Constructor_WithNegativeCoordinates_SetsSheepPosition()
    {
        // Arrange
        Point p = new Point(-5, -10);

        // Act
        EntityManager manager = new EntityManager(p);

        // Assert
        Assert.Equal(-5, manager.SheepPosition.X);
        Assert.Equal(-10, manager.SheepPosition.Y);
    }

    [Fact]
    public void Constructor_CopiesValue_NotReference()
    {
        // Arrange
        Point original = new Point(3, 4);

        // Act
        EntityManager manager = new EntityManager(original);
        // Mutate original after construction
        original.X = 99;
        original.Y = 100;

        // Assert - manager should keep the original values (struct copy semantics)
        Assert.Equal(3, manager.SheepPosition.X);
        Assert.Equal(4, manager.SheepPosition.Y);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(100, 200)]
    [InlineData(int.MaxValue, int.MinValue)]
    public void Constructor_WithVariousPoints_SetsSheepPosition(int x, int y)
    {
        // Arrange
        Point p = new Point(x, y);

        // Act
        EntityManager manager = new EntityManager(p);

        // Assert
        Assert.Equal(x, manager.SheepPosition.X);
        Assert.Equal(y, manager.SheepPosition.Y);
    }
}
