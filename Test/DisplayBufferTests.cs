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
        var buffer = new DisplayBuffer(100, 50);

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
        var buffer = new DisplayBuffer(width, height);

        // Assert
        Assert.Equal(width, buffer.Width);
        Assert.Equal(height, buffer.Height);
    }

    [Fact]
    public void SetPixel_AddsPixelToChangedPixels()
    {
        // Arrange
        var buffer = new DisplayBuffer(100, 100);
        var point = new Point(10, 20);
        var color = Color.Red;

        // Act
        var newBuffer = buffer.SetPixel(point, color);

        // Assert
        Assert.Single(newBuffer.Render());
        Assert.True(newBuffer.Render().ContainsKey(point));
        Assert.Equal(color, newBuffer.Render()[point]);
    }

    [Fact]
    public void SetPixel_ReturnsNewBuffer()
    {
        // Arrange
        var buffer = new DisplayBuffer(100, 100);
        var point = new Point(10, 20);
        var color = Color.Blue;

        // Act
        var newBuffer = buffer.SetPixel(point, color);

        // Assert
        Assert.NotSame(buffer, newBuffer);
    }

    [Fact]
    public void SetPixel_MultiplePixels_AllAreStored()
    {
        // Arrange
        var buffer = new DisplayBuffer(100, 100);
        var point1 = new Point(10, 20);
        var point2 = new Point(30, 40);
        var color1 = Color.Red;
        var color2 = Color.Green;

        // Act
        var buffer2 = buffer.SetPixel(point1, color1);
        var buffer3 = buffer2.SetPixel(point2, color2);

        // Assert
        Assert.Equal(2, buffer3.Render().Count);
        Assert.Equal(color1, buffer3.Render()[point1]);
        Assert.Equal(color2, buffer3.Render()[point2]);
    }

    [Fact]
    public void SetPixel_OverwritesExistingPixel()
    {
        // Arrange
        var buffer = new DisplayBuffer(100, 100);
        var point = new Point(10, 20);
        var color1 = Color.Red;
        var color2 = Color.Blue;

        // Act
        var buffer2 = buffer.SetPixel(point, color1);
        var buffer3 = buffer2.SetPixel(point, color2);

        // Assert
        Assert.Single(buffer3.Render());
        Assert.Equal(color2, buffer3.Render()[point]);
    }

    [Fact]
    public void Clear_FillsAllPixelsWithColor()
    {
        // Arrange
        var buffer = new DisplayBuffer(10, 10);
        var clearColor = Color.Black;

        // Act
        var clearedBuffer = buffer.Clear(clearColor);

        // Assert
        Assert.Equal(100, clearedBuffer.Render().Count); // 10 * 10
        var pixels = clearedBuffer.Render();
        for (var x = 0; x < 10; x++)
        {
            for (var y = 0; y < 10; y++)
            {
                var point = new Point(x, y);
                Assert.True(pixels.ContainsKey(point));
                Assert.Equal(clearColor, pixels[point]);
            }
        }
    }

    [Fact]
    public void Clear_WithDifferentColors()
    {
        // Arrange
        var buffer = new DisplayBuffer(5, 5);
        var color1 = Color.Red;
        var color2 = Color.Blue;

        // Act
        var clearedBuffer1 = buffer.Clear(color1);
        var clearedBuffer2 = buffer.Clear(color2);

        // Assert
        Assert.All(clearedBuffer1.Render().Values, color => Assert.Equal(color1, color));
        Assert.All(clearedBuffer2.Render().Values, color => Assert.Equal(color2, color));
    }

    [Fact]
    public void Clear_OnSmallBuffer_FillsCorrectly()
    {
        // Arrange
        var buffer = new DisplayBuffer(2, 3);
        var clearColor = Color.Green;

        // Act
        var clearedBuffer = buffer.Clear(clearColor);

        // Assert
        Assert.Equal(6, clearedBuffer.Render().Count); // 2 * 3
    }

    [Fact]
    public void Clear_ReturnsNewBuffer()
    {
        // Arrange
        var buffer = new DisplayBuffer(10, 10);

        // Act
        var clearedBuffer = buffer.Clear(Color.Black);

        // Assert
        Assert.NotSame(buffer, clearedBuffer);
    }

    [Fact]
    public void Render_ReturnsChangedPixels()
    {
        // Arrange
        var buffer = new DisplayBuffer(100, 100);
        var point = new Point(50, 50);
        var color = Color.Red;
        var bufferWithPixel = buffer.SetPixel(point, color);

        // Act
        var rendered = bufferWithPixel.Render();

        // Assert
        Assert.IsType<ImmutableDictionary<Point, Color>>(rendered);
        Assert.Single(rendered);
        Assert.Equal(color, rendered[point]);
    }

    [Fact]
    public void Render_EmptyBuffer_ReturnsEmptyDictionary()
    {
        // Arrange
        var buffer = new DisplayBuffer(100, 100);

        // Act
        var rendered = buffer.Render();

        // Assert
        Assert.Empty(rendered);
    }

    [Fact]
    public void SetPixelAndRender_ChainedOperations()
    {
        // Arrange
        var buffer = new DisplayBuffer(50, 50);
        var points = new[] { new Point(1, 1), new Point(2, 2), new Point(3, 3) };
        var color = Color.Yellow;

        // Act
        var result = buffer;
        foreach (var point in points)
        {
            result = result.SetPixel(point, color);
        }
        var rendered = result.Render();

        // Assert
        Assert.Equal(3, rendered.Count);
        foreach (var point in points)
        {
            Assert.Equal(color, rendered[point]);
        }
    }

    [Fact]
    public void ClearAndSetPixel_CombinedOperations()
    {
        // Arrange
        var buffer = new DisplayBuffer(20, 20);
        var clearColor = Color.Black;
        var pixelColor = Color.White;
        var pixelPoint = new Point(10, 10);

        // Act
        var clearedBuffer = buffer.Clear(clearColor);
        var finalBuffer = clearedBuffer.SetPixel(pixelPoint, pixelColor);

        // Assert
        Assert.Equal(401, finalBuffer.Render().Count); // (20*20) + 1 overwrite
        Assert.Equal(pixelColor, finalBuffer.Render()[pixelPoint]);
    }
}
