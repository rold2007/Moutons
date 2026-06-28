using System.Collections.Immutable;
using System.Drawing;
using GameEngine;
using Xunit;

namespace Moutons.Test;

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
    public void Constructor_ClearsBufferToBlack()
    {
        // Arrange & Act
        GameRenderer renderer = new GameRenderer(10, 10);
        ImmutableDictionary<Point, Color> rendered = renderer.Buffer.Render();

        // Assert
        Assert.NotEmpty(rendered);
        // After clear and borders, all pixels should be set
        Assert.All(rendered.Values, color =>
        {
            // Should be either black (from clear) or dark gray (from borders)
            Assert.True(color == Color.Black || color == Color.FromArgb(48, 48, 48),
                $"Unexpected color: {color}");
        });
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

    [Fact]
    public void Render_UpdatesSheepPosition()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point sheepPosition = new Point(10, 10);
        Point previousPosition = new Point(5, 5);

        // Act
        ImmutableDictionary<Point, Color> rendered = renderer.Render(sheepPosition, previousPosition);

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
        ImmutableDictionary<Point, Color> rendered = renderer.Render(sheepPosition, previousPosition);

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
        ImmutableDictionary<Point, Color> rendered = renderer.Render(sheepPosition, previousPosition);

        // Assert
        Assert.NotNull(rendered);
        // Verify it's immutable by trying to access it (would throw if mutable operations attempted)
        int count = rendered.Count;
        Assert.True(count > 0);
    }

    [Fact]
    public void Render_ResetsBufferAfterRendering()
    {
        // Arrange
        GameRenderer renderer = new GameRenderer(20, 20);
        Point sheepPosition = new Point(10, 10);
        Point previousPosition = new Point(5, 5);

        // Act
        renderer.Render(sheepPosition, previousPosition);
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
        ImmutableDictionary<Point, Color> rendered1 = renderer.Render(new Point(10, 10), new Point(5, 5));

        // Act - Second render with different position
        ImmutableDictionary<Point, Color> rendered2 = renderer.Render(new Point(15, 15), new Point(10, 10));

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
        ImmutableDictionary<Point, Color> rendered = renderer.Render(position, position);

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
        ImmutableDictionary<Point, Color> rendered = renderer.Render(sheepPosition, previousPosition);

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
        ImmutableDictionary<Point, Color> rendered = renderer.Render(sheepPosition, previousPosition);

        // Assert
        Assert.True(rendered.ContainsKey(sheepPosition));
        Assert.Equal(Color.White, rendered[sheepPosition]);
        Assert.True(rendered.ContainsKey(previousPosition));
        Assert.Equal(Color.Black, rendered[previousPosition]);
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
}
