using System.Collections.Immutable;
using System.Drawing;
using GameEngine;
using Xunit;

namespace Moutons.Test;

public class DisplayBufferTests
{
    [Fact]
    public void Constructor_SetsWidthAndHeight()
    {
        // Arrange & Act
        DisplayBuffer buffer = new DisplayBuffer(100, 50);

        // Assert
        Assert.Equal(100, buffer.Width);
        Assert.Equal(50, buffer.Height);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    [InlineData(1000, 1000)]
    public void Constructor_WithVariousDimensions_SetsCorrectly(int width, int height)
    {
        // Arrange & Act
        DisplayBuffer buffer = new DisplayBuffer(width, height);

        // Assert
        Assert.Equal(width, buffer.Width);
        Assert.Equal(height, buffer.Height);
    }

    [Fact]
    public void SetPixel_AddsPixelToChangedPixels()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(100, 100);
        Point point = new Point(10, 20);
        Color color = Color.Red;

        // Act
        DisplayBuffer newBuffer = buffer.SetPixel(point, color);

        // Assert
        Assert.Single(newBuffer.Render());
        Assert.True(newBuffer.Render().ContainsKey(point));
        Assert.Equal(color, newBuffer.Render()[point]);
    }

    [Fact]
    public void SetPixel_ReturnsNewBuffer()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(100, 100);
        Point point = new Point(10, 20);
        Color color = Color.Blue;

        // Act
        DisplayBuffer newBuffer = buffer.SetPixel(point, color);

        // Assert
        Assert.NotSame(buffer, newBuffer);
    }

    [Fact]
    public void SetPixel_MultiplePixels_AllAreStored()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(100, 100);
        Point point1 = new Point(10, 20);
        Point point2 = new Point(30, 40);
        Color color1 = Color.Red;
        Color color2 = Color.Green;

        // Act
        DisplayBuffer buffer2 = buffer.SetPixel(point1, color1);
        DisplayBuffer buffer3 = buffer2.SetPixel(point2, color2);

        // Assert
        Assert.Equal(2, buffer3.Render().Count);
        Assert.Equal(color1, buffer3.Render()[point1]);
        Assert.Equal(color2, buffer3.Render()[point2]);
    }

    [Fact]
    public void SetPixel_OverwritesExistingPixel()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(100, 100);
        Point point = new Point(10, 20);
        Color color1 = Color.Red;
        Color color2 = Color.Blue;

        // Act
        DisplayBuffer buffer2 = buffer.SetPixel(point, color1);
        DisplayBuffer buffer3 = buffer2.SetPixel(point, color2);

        // Assert
        Assert.Single(buffer3.Render());
        Assert.Equal(color2, buffer3.Render()[point]);
    }

    [Fact]
    public void Clear_FillsAllPixelsWithColor()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(10, 10);
        Color clearColor = Color.Black;

        // Act
        DisplayBuffer clearedBuffer = buffer.Clear(clearColor);

        // Assert
        Assert.Equal(100, clearedBuffer.Render().Count); // 10 * 10
        ImmutableDictionary<Point, Color> pixels = clearedBuffer.Render();
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Point point = new Point(x, y);
                Assert.True(pixels.ContainsKey(point));
                Assert.Equal(clearColor, pixels[point]);
            }
        }
    }

    [Fact]
    public void Clear_WithDifferentColors()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(5, 5);
        Color color1 = Color.Red;
        Color color2 = Color.Blue;

        // Act
        DisplayBuffer clearedBuffer1 = buffer.Clear(color1);
        DisplayBuffer clearedBuffer2 = buffer.Clear(color2);

        // Assert
        Assert.All(clearedBuffer1.Render().Values, color => Assert.Equal(color1, color));
        Assert.All(clearedBuffer2.Render().Values, color => Assert.Equal(color2, color));
    }

    [Fact]
    public void Clear_OnSmallBuffer_FillsCorrectly()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(2, 3);
        Color clearColor = Color.Green;

        // Act
        DisplayBuffer clearedBuffer = buffer.Clear(clearColor);

        // Assert
        Assert.Equal(6, clearedBuffer.Render().Count); // 2 * 3
    }

    [Fact]
    public void Clear_ReturnsNewBuffer()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(10, 10);

        // Act
        DisplayBuffer clearedBuffer = buffer.Clear(Color.Black);

        // Assert
        Assert.NotSame(buffer, clearedBuffer);
    }

    [Fact]
    public void Render_ReturnsChangedPixels()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(100, 100);
        Point point = new Point(50, 50);
        Color color = Color.Red;
        DisplayBuffer bufferWithPixel = buffer.SetPixel(point, color);

        // Act
        ImmutableDictionary<Point, Color> rendered = bufferWithPixel.Render();

        // Assert
        Assert.IsType<ImmutableDictionary<Point, Color>>(rendered);
        Assert.Single(rendered);
        Assert.Equal(color, rendered[point]);
    }

    [Fact]
    public void Render_EmptyBuffer_ReturnsEmptyDictionary()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(100, 100);

        // Act
        ImmutableDictionary<Point, Color> rendered = buffer.Render();

        // Assert
        Assert.Empty(rendered);
    }

    [Fact]
    public void SetPixelAndRender_ChainedOperations()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(50, 50);
        Point[] points = new[] { new Point(1, 1), new Point(2, 2), new Point(3, 3) };
        Color color = Color.Yellow;

        // Act
        DisplayBuffer result = buffer;
        foreach (Point point in points)
        {
            result = result.SetPixel(point, color);
        }
        ImmutableDictionary<Point, Color> rendered = result.Render();

        // Assert
        Assert.Equal(3, rendered.Count);
        foreach (Point point in points)
        {
            Assert.Equal(color, rendered[point]);
        }
    }

    [Fact]
    public void ClearAndSetPixel_CombinedOperations()
    {
        // Arrange
        DisplayBuffer buffer = new DisplayBuffer(20, 20);
        Color clearColor = Color.Black;
        Color pixelColor = Color.White;
        Point pixelPoint = new Point(10, 10);

        // Act
        DisplayBuffer clearedBuffer = buffer.Clear(clearColor);
        DisplayBuffer finalBuffer = clearedBuffer.SetPixel(pixelPoint, pixelColor);

        // Assert
        Assert.Equal(400, finalBuffer.Render().Count);
        Assert.Equal(pixelColor, finalBuffer.Render()[pixelPoint]);
    }
}
