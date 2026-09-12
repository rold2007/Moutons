using Xunit;
using Moutons.Core;

namespace Tests.Core;

public class HealthTests
{
    [Theory]
    [InlineData(100, HealthLevel.High)]
    [InlineData(67, HealthLevel.High)]
    [InlineData(66, HealthLevel.Medium)]
    [InlineData(34, HealthLevel.Medium)]
    [InlineData(33, HealthLevel.Low)]
    [InlineData(0, HealthLevel.Low)]
    public void EvaluateHealthLevel_FromPercent_ReturnsExpected(int percent, HealthLevel expected)
    {
        // Act
        HealthLevel actual = Health.EvaluateHealthLevel(percent);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(100, 100, HealthLevel.High)]
    [InlineData(67, 100, HealthLevel.High)]
    [InlineData(50, 100, HealthLevel.Medium)]
    [InlineData(33, 100, HealthLevel.Low)]
    [InlineData(-10, 100, HealthLevel.Low)]
    [InlineData(150, 100, HealthLevel.High)]
    public void EvaluateHealthLevel_FromAbsoluteValues_ReturnsExpected(int health, int max, HealthLevel expected)
    {
        // Act
        HealthLevel actual = Health.EvaluateHealthLevel(health, max);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void EvaluateHealthLevel_FromAbsoluteValues_MidpointRoundDown_ToEven_ReturnsMedium()
    {
        // Arrange
        int health = 665; // 665/1000 = 66.5 -> Math.Round(66.5) = 66 (ToEven)
        int max = 1000;

        // Act
        HealthLevel actual = Health.EvaluateHealthLevel(health, max);

        // Assert
        Assert.Equal(HealthLevel.Medium, actual);
    }

    [Fact]
    public void EvaluateHealthLevel_FromAbsoluteValues_MidpointRoundUp_ToEven_ReturnsHigh()
    {
        // Arrange
        int health = 675; // 675/1000 = 67.5 -> Math.Round(67.5) = 68 (ToEven)
        int max = 1000;

        // Act
        HealthLevel actual = Health.EvaluateHealthLevel(health, max);

        // Assert
        Assert.Equal(HealthLevel.High, actual);
    }

    [Fact]
    public void EvaluateHealthLevel_FromAbsoluteValues_ZeroMax_ReturnsLow()
    {
        // Arrange
        int health = 50;
        int max = 0;

        // Act
        HealthLevel actual = Health.EvaluateHealthLevel(health, max);

        // Assert
        Assert.Equal(HealthLevel.Low, actual);
    }

}
