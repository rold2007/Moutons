using System.Drawing;
using Moutons.Core;
using Xunit;

namespace Tests.Core
{
    public class GameLogicTests
    {
        [Fact]
        public void SheepPosition_Property_ReturnsInitialPosition()
        {
            // Arrange
            var initial = new Point(2, 3);
            var sut = new GameLogic(10, 10, initial);

            // Act
            var result = sut.SheepPosition;

            // Assert
            Assert.Equal(initial, result);
        }

        [Fact]
        public void PreviousSheepPosition_Property_ReturnsInitialPosition()
        {
            // Arrange
            var initial = new Point(4, 5);
            var sut = new GameLogic(8, 8, initial);

            // Act
            var result = sut.PreviousSheepPosition;

            // Assert
            Assert.Equal(initial, result);
        }

        [Fact]
        public void SheepPositionChanged_WhenPositionsAreSame_ReturnsFalse()
        {
            // Arrange
            var initial = new Point(1, 1);
            var sut = new GameLogic(5, 5, initial);

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
            var sut = new GameLogic(10, 10, initial);

            // Act
            var moved = sut.MoveSheep(Direction.Right);

            // Assert
            Assert.True(moved.SheepPositionChanged);
            // also verify previous position preserved and differs from current
            Assert.Equal(initial, moved.PreviousSheepPosition);
            Assert.NotEqual(moved.PreviousSheepPosition, moved.SheepPosition);
        }

        [Fact]
        public void StateChanged_MirrorsSheepPositionChanged()
        {
            // Arrange
            var initial = new Point(3, 3);
            var sut = new GameLogic(7, 7, initial);

            // Act
            var moved = sut.MoveSheep(Direction.Left);

            // Assert
            // StateChanged should reflect the same boolean as SheepPositionChanged
            Assert.Equal(moved.SheepPositionChanged, moved.StateChanged);
        }

        [Fact]
        public void Constructor_InitializesBothStatesToInitialPosition()
        {
            // Arrange
            var initial = new Point(6, 6);

            // Act
            var sut = new GameLogic(12, 12, initial);

            // Assert
            Assert.Equal(initial, sut.SheepPosition);
            Assert.Equal(initial, sut.PreviousSheepPosition);
            Assert.False(sut.SheepPositionChanged);
            Assert.False(sut.StateChanged);
        }

        [Fact]
        public void MoveSheep_Left_AtLeftEdge_ClampsToMinimumAndPreservesPrevious()
        {
            // Arrange
            var initial = new Point(1, 2);
            var sut = new GameLogic(10, 10, initial);

            // Act
            var moved = sut.MoveSheep(Direction.Left);

            // Assert
            Assert.Equal(new Point(1, 2), moved.SheepPosition); // clamped to min X = 1
            Assert.Equal(initial, moved.PreviousSheepPosition);
            Assert.False(sut.SheepPositionChanged); // original not mutated
        }

        [Fact]
        public void MoveSheep_Right_AtRightEdge_ClampsToMaximumAndPreservesPrevious()
        {
            // Arrange
            int width = 5; // max allowed X = width - 2 -> 3
            var initial = new Point(3, 4);
            var sut = new GameLogic(width, 10, initial);

            // Act
            var moved = sut.MoveSheep(Direction.Right);

            // Assert
            Assert.Equal(new Point(3, 4), moved.SheepPosition); // clamped to max X = 3
            Assert.Equal(initial, moved.PreviousSheepPosition);
            Assert.False(sut.SheepPositionChanged);
        }

        [Fact]
        public void MoveSheep_Up_AtTopEdge_ClampsToMinimumY()
        {
            // Arrange
            var initial = new Point(5, 1);
            var sut = new GameLogic(10, 10, initial);

            // Act
            var moved = sut.MoveSheep(Direction.Up);

            // Assert
            Assert.Equal(new Point(5, 1), moved.SheepPosition); // clamped to min Y = 1
            Assert.Equal(initial, moved.PreviousSheepPosition);
        }

        [Fact]
        public void MoveSheep_Down_AtBottomEdge_ClampsToMaximumY()
        {
            // Arrange
            int height = 6; // max allowed Y = height - 2 -> 4
            var initial = new Point(2, 4);
            var sut = new GameLogic(10, height, initial);

            // Act
            var moved = sut.MoveSheep(Direction.Down);

            // Assert
            Assert.Equal(new Point(2, 4), moved.SheepPosition); // clamped to max Y = 4
            Assert.Equal(initial, moved.PreviousSheepPosition);
        }

        [Fact]
        public void MoveSheep_InvalidDirection_ThrowsArgumentOutOfRangeException_AndDoesNotMutateOriginal()
        {
            // Arrange
            var initial = new Point(4, 4);
            var sut = new GameLogic(10, 10, initial);

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => sut.MoveSheep((Direction)999));
            Assert.Contains("Unsupported direction.", ex.Message);

            // original instance must remain unchanged
            Assert.Equal(initial, sut.SheepPosition);
            Assert.False(sut.SheepPositionChanged);
        }

    }
}
