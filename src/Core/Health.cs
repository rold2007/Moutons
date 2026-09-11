using System;

namespace Moutons.Core;

public enum HealthLevel
{
    High,
    Medium,
    Low
}

public static class Health
{
    // Evaluate a health percentage (0-100) and return a HealthLevel.
    public static HealthLevel EvaluateHealthLevel(int healthPercent)
    {
        return healthPercent switch
        {
            >= 67 => HealthLevel.High,
            >= 34 => HealthLevel.Medium,
            _ => HealthLevel.Low
        };
    }

    // Convenience overload: evaluate from absolute health and maxHealth.
    public static HealthLevel EvaluateHealthLevel(int health, int maxHealth)
    {
        int clamped = Math.Clamp(health, 0, maxHealth);
        int percent = (int)Math.Round(clamped / (double)maxHealth * 100);
        return EvaluateHealthLevel(percent);
    }
}
