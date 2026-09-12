using System.Collections.Immutable;
using System.Collections.Generic;
using System.Drawing;
using GameEngine;
using Moutons.Core;
using Xunit;

namespace Tests.GameEngine;

public class GameRendererTests
{
    [Fact]
    public void Constructor_InitializesBufferWithCorrectDimensions()
    {
        // Arrange & Act
        GameRenderer renderer = new GameRenderer(100, 50);

        // Assert
        Assert.NotNull(renderer.Buffer);
        Assert.Equal(100, renderer.Buffer.Width);
        Assert.Equal(50, renderer.Buffer.Height);
    }

    [Fact]
    public void Constructor_InitializesBlackBackgroundAndGrayBorders()
    {
        // Arrange & Act
        GameRenderer renderer = new GameRenderer(10, 10);
        ImmutableDictionary<Point, Color> rendered = renderer.Buffer.Render();
        Color borderColor = Color.FromArgb(48, 48, 48);

        // Assert
        Assert.Equal(100, rendered.Count);

        for (int x = 0; x < 10; x++)
        {
            Assert.Equal(borderColor, rendered[new Point(x, 0)]);
            Assert.Equal(borderColor, rendered[new Point(x, 9)]);
        }

        for (int y = 1; y < 9; y++)
        {
            Assert.Equal(borderColor, rendered[new Point(0, y)]);
            Assert.Equal(borderColor, rendered[new Point(9, y)]);
        }

        for (int x = 1; x < 9; x++)
        {
            for (int y = 1; y < 9; y++)
            {
                Assert.Equal(Color.Black, rendered[new Point(x, y)]);
            }
        }
    }

    [Fact]
    public void Constructor_DrawsBorders()
    {
        // Arrange & Act
        GameRenderer renderer = new GameRenderer(10, 10);
        ImmutableDictionary<Point, Color> rendered = renderer.Buffer.Render();
        Color borderColor = Color.FromArgb(48, 48, 48);

        // Assert - check top border
        for (int x = 0; x < 10; x++)
        {
            Point topBorderPixel = new Point(x, 0);
            Assert.True(rendered.ContainsKey(topBorderPixel));
            Assert.Equal(borderColor, rendered[topBorderPixel]);
        }

        // Assert - check bottom border
        for (int x = 0; x < 10; x++)
        {
            Point bottomBorderPixel = new Point(x, 9);
            Assert.True(rendered.ContainsKey(bottomBorderPixel));
            Assert.Equal(borderColor, rendered[bottomBorderPixel]);
        }

        // Assert - check left border
        for (int y = 1; y < 10; y++)
        {
            Point leftBorderPixel = new Point(0, y);
            Assert.True(rendered.ContainsKey(leftBorderPixel));
            Assert.Equal(borderColor, rendered[leftBorderPixel]);
        }

        // Assert - check right border
        for (int y = 1; y < 10; y++)
        {
            Point rightBorderPixel = new Point(9, y);
            Assert.True(rendered.ContainsKey(rightBorderPixel));
            Assert.Equal(borderColor, rendered[rightBorderPixel]);
        }
    }

    [Theory]
    [InlineData(50, 30)]
    [InlineData(100, 100)]
    [InlineData(20, 20)]
    public void Constructor_WithVariousDimensions_InitializesCorrectly(int width, int height)
    {
        // Arrange & Act
        GameRenderer renderer = new GameRenderer(width, height);

        // Assert
        Assert.Equal(width, renderer.Buffer.Width);
        Assert.Equal(height, renderer.Buffer.Height);
    }

    [Fact]
    public void DrawBorders_AddsPixelsToBuffer()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(10, 10);
        GameRenderer renderer = new GameRenderer(10, 10);
        Color borderColor = Color.Red;

        // Act
        DisplayBuffer borderedBuffer = renderer.DrawBorders(buffer, borderColor);

        // Assert
        Assert.NotNull(borderedBuffer);
        ImmutableDictionary<Point, Color> rendered = borderedBuffer.Render();

        // Top border: 10 pixels
        // Bottom border: 10 pixels
        // Left border (excluding corners): 8 pixels
        // Right border (excluding corners): 8 pixels
        // Total: 36 pixels
        Assert.Equal(36, rendered.Count);

        for (int x = 0; x < 10; x++)
        {
            Assert.Equal(borderColor, rendered[new Point(x, 0)]);
            Assert.Equal(borderColor, rendered[new Point(x, 9)]);
        }

        for (int y = 1; y < 9; y++)
        {
            Assert.Equal(borderColor, rendered[new Point(0, y)]);
            Assert.Equal(borderColor, rendered[new Point(9, y)]);
        }

        Assert.False(rendered.ContainsKey(new Point(5, 5)));
    }

    [Fact]
    public void DrawBorders_WithCustomColor()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(10, 10);
        GameRenderer renderer = new GameRenderer(10, 10);
        Color customColor = Color.Blue;

        // Act
        DisplayBuffer borderedBuffer = renderer.DrawBorders(buffer, customColor);

        // Assert
        ImmutableDictionary<Point, Color> rendered = borderedBuffer.Render();
        Assert.All(rendered.Values, color => Assert.Equal(customColor, color));
    }

    [Fact]
    public void DrawBorders_ReturnsNewBuffer()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(10, 10);
        GameRenderer renderer = new GameRenderer(10, 10);

        // Act
        DisplayBuffer borderedBuffer = renderer.DrawBorders(buffer, Color.Green);

        // Assert
        Assert.NotSame(buffer, borderedBuffer);
    }

    [Fact]
    public void DrawBorders_SmallBuffer_DrawsCorrectly()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(3, 3);
        GameRenderer renderer = new GameRenderer(3, 3);
        Color borderColor = Color.Yellow;

        // Act
        DisplayBuffer borderedBuffer = renderer.DrawBorders(buffer, borderColor);

        // Assert
        ImmutableDictionary<Point, Color> rendered = borderedBuffer.Render();
        // For 3x3: top 3 + bottom 3 + left (1 middle) + right (1 middle) = 8 pixels
        Assert.Equal(8, rendered.Count);
    }

    [Fact]
    public void DrawBorders_OnLargeBuffer()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(100, 50);
        GameRenderer renderer = new GameRenderer(100, 50);
        Color borderColor = Color.Black;

        // Act
        DisplayBuffer borderedBuffer = renderer.DrawBorders(buffer, borderColor);

        // Assert
        ImmutableDictionary<Point, Color> rendered = borderedBuffer.Render();
        // Top: 100 + Bottom: 100 + Left: 48 + Right: 48 = 296
        Assert.Equal(296, rendered.Count);
    }

    [Theory]
    [InlineData(4, 4)]
    [InlineData(7, 3)]
    [InlineData(20, 10)]
    public void DrawBorders_AllBoundaryCoordinates_HaveBorderColor(int width, int height)
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(width, height);
        GameRenderer renderer = new GameRenderer(width, height);
        Color borderColor = Color.Magenta;

        // Act
        DisplayBuffer borderedBuffer = renderer.DrawBorders(buffer, borderColor);
        ImmutableDictionary<Point, Color> rendered = borderedBuffer.Render();

        // Assert
        for (int x = 0; x < width; x++)
        {
            Assert.Equal(borderColor, rendered[new Point(x, 0)]);
            Assert.Equal(borderColor, rendered[new Point(x, height - 1)]);
        }

        for (int y = 1; y < height; y++)
        {
            Assert.Equal(borderColor, rendered[new Point(0, y)]);
            Assert.Equal(borderColor, rendered[new Point(width - 1, y)]);
        }
    }

    [Fact]
    public void Render_UpdatesSheepPosition()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point sheepPosition = new Point(10, 10);
        Point previousPosition = new Point(5, 5);

        // Act
        ImmutableDictionary<Point, Color> rendered = renderer.Render(CreateGameLogic(previousPosition), CreateGameLogic(sheepPosition));

        // Assert
        Assert.NotNull(rendered);
        Assert.True(rendered.ContainsKey(sheepPosition));
        Assert.Equal(Color.White, rendered[sheepPosition]);
    }

    [Fact]
    public void Render_ClearsPreviousSheepPosition()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point sheepPosition = new Point(10, 10);
        Point previousPosition = new Point(5, 5);

        // Act
        ImmutableDictionary<Point, Color> rendered = renderer.Render(CreateGameLogic(previousPosition), CreateGameLogic(sheepPosition));

        // Assert
        Assert.True(rendered.ContainsKey(previousPosition));
        Assert.Equal(Color.Black, rendered[previousPosition]);
    }

    [Fact]
    public void Render_ReturnsImmutableDictionary()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point sheepPosition = new Point(10, 10);
        Point previousPosition = new Point(5, 5);

        // Act
        ImmutableDictionary<Point, Color> rendered = renderer.Render(CreateGameLogic(previousPosition), CreateGameLogic(sheepPosition));
        IDictionary<Point, Color> asDictionary = rendered;

        // Assert
        Assert.Throws<NotSupportedException>(() => asDictionary.Add(new Point(0, 0), Color.White));
    }

    [Fact]
    public void Render_ResetsBufferAfterRendering()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point sheepPosition = new Point(10, 10);
        Point previousPosition = new Point(5, 5);

        // Act
        renderer.Render(CreateGameLogic(previousPosition), CreateGameLogic(sheepPosition));
        ImmutableDictionary<Point, Color> bufferAfterRender = renderer.Buffer.Render();

        // Assert
        // Buffer should be reset (empty) after render
        Assert.Empty(bufferAfterRender);
    }

    [Fact]
    public void Render_MultipleCalls_WithDifferentPositions()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(30, 30);

        // Act - First render
        ImmutableDictionary<Point, Color> rendered1 = renderer.Render(CreateGameLogic(new Point(5, 5)), CreateGameLogic(new Point(10, 10)));

        // Act - Second render with different position
        ImmutableDictionary<Point, Color> rendered2 = renderer.Render(CreateGameLogic(new Point(10, 10)), CreateGameLogic(new Point(15, 15)));

        // Assert
        Assert.NotNull(rendered1);
        Assert.NotNull(rendered2);
        Assert.True(rendered1.ContainsKey(new Point(10, 10)));
        Assert.True(rendered2.ContainsKey(new Point(15, 15)));
    }

    [Fact]
    public void Render_SamePositionForBothSheepAndPrevious()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point position = new Point(10, 10);

        // Act
        ImmutableDictionary<Point, Color> rendered = renderer.Render(CreateGameLogic(position), CreateGameLogic(position));

        // Assert
        // When previous and current are the same, both operations happen
        // First: clear to black, then: set to white
        Assert.True(rendered.ContainsKey(position));
        Assert.Equal(Color.White, rendered[position]);
    }

    [Fact]
    public void Render_WithZeroCoordinates()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point sheepPosition = new Point(0, 0);
        Point previousPosition = new Point(1, 1);

        // Act
        ImmutableDictionary<Point, Color> rendered = renderer.Render(CreateGameLogic(previousPosition), CreateGameLogic(sheepPosition));

        // Assert
        Assert.True(rendered.ContainsKey(sheepPosition));
        Assert.Equal(Color.White, rendered[sheepPosition]);
        Assert.True(rendered.ContainsKey(previousPosition));
        Assert.Equal(Color.Black, rendered[previousPosition]);
    }

    [Fact]
    public void Render_WithEdgeCoordinates()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point sheepPosition = new Point(19, 19);
        Point previousPosition = new Point(0, 19);

        // Act
        ImmutableDictionary<Point, Color> rendered = renderer.Render(CreateGameLogic(previousPosition), CreateGameLogic(sheepPosition));

        // Assert
        Assert.True(rendered.ContainsKey(sheepPosition));
        Assert.Equal(Color.White, rendered[sheepPosition]);
        Assert.True(rendered.ContainsKey(previousPosition));
        Assert.Equal(Color.Black, rendered[previousPosition]);
    }

    [Theory]
    [InlineData(20, 20, 0, 0, 1, 1)]
    [InlineData(20, 20, 19, 0, 18, 0)]
    [InlineData(20, 20, 0, 19, 0, 18)]
    [InlineData(20, 20, 19, 19, 18, 19)]
    [InlineData(20, 20, 10, 0, 10, 1)]
    [InlineData(20, 20, 10, 19, 10, 18)]
    [InlineData(20, 20, 0, 10, 1, 10)]
    [InlineData(20, 20, 19, 10, 18, 10)]
    public void Render_BoundaryCoordinates_UpdatesCurrentAndPreviousPixels(
        int width,
        int height,
        int sheepX,
        int sheepY,
        int previousX,
        int previousY)
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(width, height);
        Point sheepPosition = new Point(sheepX, sheepY);
        Point previousPosition = new Point(previousX, previousY);

        // Act
        ImmutableDictionary<Point, Color> rendered = renderer.Render(CreateGameLogic(previousPosition), CreateGameLogic(sheepPosition));

        // Assert
        Assert.Equal(Color.White, rendered[sheepPosition]);
        Assert.Equal(Color.Black, rendered[previousPosition]);
    }

    private static GameLogic CreateGameLogic(Point position)
    {
        return new GameLogic(20, 20, position);
    }

    [Fact]
    public void BufferProperty_CanBeAccessed()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);

        // Act
        DisplayBuffer buffer = renderer.Buffer;

        // Assert
        Assert.NotNull(buffer);
        Assert.Equal(20, buffer.Width);
        Assert.Equal(20, buffer.Height);
    }

    [Fact]
    public void BufferProperty_IsDisplayBuffer()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);

        // Act
        DisplayBuffer buffer = renderer.Buffer;

        // Assert
        Assert.IsType<DisplayBuffer>(buffer);
    }
    [Fact]
    public void DrawBorders_WidthOne_SetsEntireColumnToBorder()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(1, 5);
        GameRenderer renderer = new GameRenderer(1, 5);
        Color borderColor = Color.FromArgb(123, 45, 67);

        // Act
        DisplayBuffer bordered = renderer.DrawBorders(buffer, borderColor);

        // Assert
        ImmutableDictionary<Point, Color> rendered = bordered.Render();
        Assert.Equal(5, rendered.Count);

        for (int y = 0; y < 5; y++)
        {
            Assert.Equal(borderColor, rendered[new Point(0, y)]);
        }
    }

    [Fact]
    public void DrawBorders_HeightOne_SetsEntireRowToBorder()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(5, 1);
        GameRenderer renderer = new GameRenderer(5, 1);
        Color borderColor = Color.FromArgb(10, 20, 30);

        // Act
        DisplayBuffer bordered = renderer.DrawBorders(buffer, borderColor);

        // Assert
        ImmutableDictionary<Point, Color> rendered = bordered.Render();
        Assert.Equal(5, rendered.Count);

        for (int x = 0; x < 5; x++)
        {
            Assert.Equal(borderColor, rendered[new Point(x, 0)]);
        }
    }

    [Fact]
    public void DrawBorders_OneByOne_SetsSinglePixelToBorder()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(1, 1);
        GameRenderer renderer = new GameRenderer(1, 1);
        Color borderColor = Color.Lime;

        // Act
        DisplayBuffer bordered = renderer.DrawBorders(buffer, borderColor);

        // Assert
        ImmutableDictionary<Point, Color> rendered = bordered.Render();
        Assert.Single(rendered);
        Assert.Equal(borderColor, rendered[new Point(0, 0)]);
    }

    [Fact]
    public void Constructor_WithOneByOne_InitializesBorderColor()
    {
        // Arrange & Act
        GameRenderer renderer = new GameRenderer(1, 1);

        // Assert
        ImmutableDictionary<Point, Color> rendered = renderer.Buffer.Render();
        Assert.Single(rendered);
        Assert.Equal(Color.FromArgb(48, 48, 48), rendered[new Point(0, 0)]);
    }

    [Fact]
    public void Render_OnOneByOne_ReturnsWhiteAndResetsBuffer()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(1, 1);
        Point prev = new Point(0, 0);
        Point cur = new Point(0, 0);

        // Act
        ImmutableDictionary<Point, Color> changed = renderer.Render(new GameLogic(1, 1, prev), new GameLogic(1, 1, cur));

        // Assert - changed should contain the single pixel as white
        Assert.Single(changed);
        Assert.Equal(Color.White, changed[new Point(0, 0)]);

        // Buffer must be reset after render
        ImmutableDictionary<Point, Color> after = renderer.Buffer.Render();
        Assert.Empty(after);
    }

}
