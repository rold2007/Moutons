using System.Drawing;
using Moutons.Core;
using Xunit;

namespace Tests.Core;

public class EntityManagerTests
{
    [Fact]
    public void Constructor_StoresSheepAndGrassPositions()
    {
        // Arrange
        Point sheep = new Point(2, 3);
        Point grass = new Point(4, 5);
        var sheepPositions = System.Collections.Immutable.ImmutableHashSet.Create(sheep);
        var grassPositions = System.Collections.Immutable.ImmutableHashSet.Create(grass);

        // Act
        EntityManager manager = new EntityManager(sheepPositions, grassPositions);

        // Assert
        Assert.Equal(sheepPositions, manager.SheepPositions);
        Assert.Equal(grassPositions, manager.GrassPositions);
    }
}
