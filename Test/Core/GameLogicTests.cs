using System.Drawing;
using Moutons.Core;
using Xunit;

namespace Tests.Core
{
    public class GameLogicTests
    {
        [Fact]
        public void SheepPositions_Property_ReturnsInitialPositions()
        {
            // Arrange
            var initial = new Point(2, 3);
            var sut = new GameLogic(10, 10).SpawnSheep(initial);

            // Act
            var result = sut.SheepPositions;

            // Assert
            Assert.Equal(new[] { initial }, result);
        }

        [Fact]
        public void PreviousSheepPositions_Property_IsEmptyBeforeAnyPreviousState()
        {
            // Arrange
            var initial = new Point(4, 5);
            var sut = new GameLogic(8, 8).SpawnSheep(initial);

            // Act
            var result = sut.PreviousSheepPositions;

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void SpawnGrass_AddsGrassAndPreservesItWhenSheepMoves()
        {
            // Arrange
            var sheep = new Point(1, 1);
            var grass = new Point(5, 5);
            var sut = new GameLogic(10, 10).SpawnSheep(sheep).SpawnGrass(grass);

            // Act
            var moved = sut.MoveSheep(Direction.Right);

            // Assert
            Assert.Contains(grass, sut.GrassPositions);
            Assert.Equal(sut.GrassPositions, moved.GrassPositions);
        }

        [Fact]
        public void SheepPositionChanged_WhenPositionsAreSame_ReturnsFalse()
        {
            // Arrange
            var initial = new Point(1, 1);
            var sut = new GameLogic(5, 5).SpawnSheep(initial).SpawnSheep(initial);

            // Act
            var changed = sut.SheepPositionChanged;

            // Assert
            Assert.False(changed);
        }

        [Fact]
        public void SheepPositionChanged_WhenSheepHasMoved_ReturnsTrue()
        {
            // Arrange
            var initial = new Point(5, 5);
            var sut = new GameLogic(10, 10).SpawnSheep(initial);

            // Act
            var moved = sut.MoveSheep(Direction.Right);

            // Assert
            Assert.True(moved.SheepPositionChanged);
            Assert.Equal(new[] { initial }, moved.PreviousSheepPositions);
            Assert.Contains(new Point(6, 5), moved.SheepPositions);
        }

        [Fact]
        public void StateChanged_MirrorsSheepPositionChanged()
        {
            // Arrange
            var initial = new Point(3, 3);
            var sut = new GameLogic(7, 7).SpawnSheep(initial);

            // Act
            var moved = sut.MoveSheep(Direction.Left);

            // Assert
            // StateChanged should reflect the same boolean as SheepPositionChanged
            Assert.Equal(moved.SheepPositionChanged, moved.StateChanged);
        }

        [Fact]
        public void SpawnSheep_AddsPositionAndReportsStateChange()
        {
            // Arrange
            var initial = new Point(6, 6);
            var empty = new GameLogic(12, 12);

            // Act
            var sut = empty.SpawnSheep(initial);

            // Assert
            Assert.Equal(new[] { initial }, sut.SheepPositions);
            Assert.Empty(sut.PreviousSheepPositions);
            Assert.True(sut.SheepPositionChanged);
            Assert.True(sut.StateChanged);
        }

        [Fact]
        public void MoveSheep_Left_AtLeftEdge_ClampsToMinimumAndPreservesPrevious()
        {
            // Arrange
            var initial = new Point(1, 2);
            var sut = new GameLogic(10, 10).SpawnSheep(initial);

            // Act
            var moved = sut.MoveSheep(Direction.Left);

            // Assert
            Assert.Equal(new[] { initial }, moved.SheepPositions); // clamped to min X = 1
            Assert.Equal(new[] { initial }, moved.PreviousSheepPositions);
            Assert.False(moved.SheepPositionChanged);
            Assert.True(sut.SheepPositionChanged); // original instance remains at its spawn state
        }

        [Fact]
        public void MoveSheep_Right_AtRightEdge_ClampsToMaximumAndPreservesPrevious()
        {
            // Arrange
            int width = 5; // max allowed X = width - 2 -> 3
            var initial = new Point(3, 4);
            var sut = new GameLogic(width, 10).SpawnSheep(initial);

            // Act
            var moved = sut.MoveSheep(Direction.Right);

            // Assert
            Assert.Equal(new[] { initial }, moved.SheepPositions); // clamped to max X = 3
            Assert.Equal(new[] { initial }, moved.PreviousSheepPositions);
            Assert.False(moved.SheepPositionChanged);
        }

        [Fact]
        public void MoveSheep_Up_AtTopEdge_ClampsToMinimumY()
        {
            // Arrange
            var initial = new Point(5, 1);
            var sut = new GameLogic(10, 10).SpawnSheep(initial);

            // Act
            var moved = sut.MoveSheep(Direction.Up);

            // Assert
            Assert.Equal(new[] { initial }, moved.SheepPositions); // clamped to min Y = 1
            Assert.Equal(new[] { initial }, moved.PreviousSheepPositions);
            Assert.False(moved.SheepPositionChanged);
        }

        [Fact]
        public void MoveSheep_Down_AtBottomEdge_ClampsToMaximumY()
        {
            // Arrange
            int height = 6; // max allowed Y = height - 2 -> 4
            var initial = new Point(2, 4);
            var sut = new GameLogic(10, height).SpawnSheep(initial);

            // Act
            var moved = sut.MoveSheep(Direction.Down);

            // Assert
            Assert.Equal(new[] { initial }, moved.SheepPositions); // clamped to max Y = 4
            Assert.Equal(new[] { initial }, moved.PreviousSheepPositions);
            Assert.False(moved.SheepPositionChanged);
        }

        [Fact]
        public void MoveSheep_MovesAllSheepAndPreservesTheirCount()
        {
            // Arrange
            var firstSheep = new Point(2, 2);
            var secondSheep = new Point(4, 4);
            var sut = new GameLogic(10, 10).SpawnSheep(firstSheep).SpawnSheep(secondSheep);

            // Act
            var moved = sut.MoveSheep(Direction.Right);

            // Assert
            Assert.Equal(new[] { new Point(3, 2), new Point(5, 4) }.ToHashSet(), moved.SheepPositions);
            Assert.Equal(2, moved.SheepPositions.Count);
            Assert.Equal(sut.SheepPositions, moved.PreviousSheepPositions);
        }

        [Fact]
        public void MoveSheep_InvalidDirection_ThrowsArgumentOutOfRangeException_AndDoesNotMutateOriginal()
        {
            // Arrange
            var initial = new Point(4, 4);
            var sut = new GameLogic(10, 10).SpawnSheep(initial);

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => sut.MoveSheep((Direction)999));
            Assert.Contains("Unsupported direction.", ex.Message);

            // original instance must remain unchanged
            Assert.Equal(new[] { initial }, sut.SheepPositions);
            Assert.True(sut.SheepPositionChanged);
        }

    }
}
