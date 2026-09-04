using System;

namespace GameConsole;

public static class ConsoleUtils
{
    public static string GetHealthColor(int healthPercent)
    {
        return healthPercent switch
        {
            >= 67 => "green",
            >= 34 => "yellow",
            _ => "red"
        };
    }
}
