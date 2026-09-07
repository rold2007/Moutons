using Xunit;
using Moutons;

namespace Moutons.Test;

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
}
