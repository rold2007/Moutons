using System;
using System.Collections.Generic;
using System.Drawing;
using Moutons.UI;
using Xunit;
using DrawingColor = System.Drawing.Color;

namespace Tests.UI
{
    public class ConsoleUtilsTests
    {
        [Fact]
        public void GetHealthColor_High_ReturnsGreen()
        {
            // Act
            string result = ConsoleUtils.GetHealthColor(Moutons.Core.HealthLevel.High);

            // Assert
            Assert.Equal("green", result);
        }

        [Fact]
        public void GetHealthColor_Medium_ReturnsYellow()
        {
            // Act
            string result = ConsoleUtils.GetHealthColor(Moutons.Core.HealthLevel.Medium);

            // Assert
            Assert.Equal("yellow", result);
        }

        [Fact]
        public void GetHealthColor_Low_ReturnsRed()
        {
            // Act
            string result = ConsoleUtils.GetHealthColor(Moutons.Core.HealthLevel.Low);

            // Assert
            Assert.Equal("red", result);
        }

        [Fact]
        public void GetHealthColor_UnknownValue_ReturnsRed()
        {
            // Arrange
            var unknown = (Moutons.Core.HealthLevel)999;

            // Act
            string result = ConsoleUtils.GetHealthColor(unknown);

            // Assert
            Assert.Equal("red", result);
        }

        [Fact]
        public void BuildHealthBarMarkup_FullHealth_ReturnsAllFilledAndGreen100()
        {
            // Arrange
            int health = 100;
            int max = 100;
            int width = 10;

            int clamped = Math.Clamp(health, 0, max);
            int percent = (int)Math.Round(clamped / (double)max * 100);
            int filled = (int)Math.Round(percent / 100.0 * width);
            int empty = width - filled;
            string fill = System.Net.WebUtility.HtmlEncode(new string('#', filled)); // independent escape substitute (not used by production)
            string rest = System.Net.WebUtility.HtmlEncode(new string('-', empty));

            // Act
            string markup = ConsoleUtils.BuildHealthBarMarkup(health, max, width);

            // Assert
            // Verify percent, color and number of fill/empty characters
            Assert.Contains("[green]", markup);
            Assert.Contains(new string('#', filled), markup);
            Assert.Contains(new string('-', empty), markup);
            Assert.EndsWith($" {percent,3}%[/]", markup);
        }

        [Fact]
        public void BuildHealthBarMarkup_ZeroHealth_ReturnsAllEmptyAndRed0()
        {
            // Arrange
            int health = 0;
            int max = 100;
            int width = 5;

            int clamped = Math.Clamp(health, 0, max);
            int percent = (int)Math.Round(clamped / (double)max * 100);
            int filled = (int)Math.Round(percent / 100.0 * width);
            int empty = width - filled;

            // Act
            string markup = ConsoleUtils.BuildHealthBarMarkup(health, max, width);

            // Assert
            Assert.Contains("[red]", markup);
            Assert.DoesNotContain("#", markup);
            Assert.Contains(new string('-', empty), markup);
            Assert.EndsWith($" {percent,3}%[/]", markup);
        }

        [Fact]
        public void BuildHealthBarMarkup_ClampAboveMaxAndBelowZero_WorksAsExpected()
        {
            // Arrange
            int over = 150;
            int under = -10;
            int max = 100;
            int width = 4;

            // Act
            string overMarkup = ConsoleUtils.BuildHealthBarMarkup(over, max, width);
            string underMarkup = ConsoleUtils.BuildHealthBarMarkup(under, max, width);

            // Assert
            // over should be full
            Assert.Contains("[green]", overMarkup);
            Assert.Contains(new string('#', width), overMarkup);
            Assert.EndsWith(" 100%[/]", overMarkup);

            // under should be empty
            Assert.Contains("[red]", underMarkup);
            Assert.DoesNotContain("#", underMarkup);
            Assert.EndsWith("   0%[/]", underMarkup);
        }

        [Fact]
        public void BuildHealthBarMarkup_ZeroMax_DoesNotThrow_ProducesZeroPercent()
        {
            // Arrange
            int health = 50;
            int max = 0;
            int width = 3;

            // Act
            string markup = ConsoleUtils.BuildHealthBarMarkup(health, max, width);

            // Assert
            // With a max of zero the implementation ends up producing 0 percent in existing code
            Assert.Contains("[red]", markup);
            Assert.DoesNotContain("#", markup);
            Assert.EndsWith("   0%[/]", markup);
        }

        [Fact]
        public void BuildCanvasMarkup_With2x2Pixels_ReturnsExpectedMarkup()
        {
            // Arrange
            int width = 2;
            int height = 2;
            var pixels = new Dictionary<Point, DrawingColor>
            {
                [new Point(0,0)] = DrawingColor.FromArgb(1,2,3),
                [new Point(1,0)] = DrawingColor.FromArgb(4,5,6),
                [new Point(0,1)] = DrawingColor.FromArgb(7,8,9),
                [new Point(1,1)] = DrawingColor.FromArgb(10,11,12),
            };

            string expectedRow0 = $"[on rgb(1,2,3)]  [/]" + $"[on rgb(4,5,6)]  [/]";
            string expectedRow1 = $"[on rgb(7,8,9)]  [/]" + $"[on rgb(10,11,12)]  [/]";
            string expected = expectedRow0 + Environment.NewLine + expectedRow1;

            // Act
            string actual = ConsoleUtils.BuildCanvasMarkup(width, height, pixels);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
