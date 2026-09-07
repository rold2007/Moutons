using Spectre.Console;
using Moutons;

namespace GameConsole;

public static class ConsoleUtils
{
   public static string GetHealthColor(HealthLevel level)
   {
      return level switch
      {
         HealthLevel.High => "green",
         HealthLevel.Medium => "yellow",
         HealthLevel.Low => "red",
         _ => "red"
      };
   }


   public static string BuildHealthBarMarkup(int health, int maxHealthValue, int width)
   {
      int clampedHealth = Math.Clamp(health, 0, maxHealthValue);
      int percent = (int)Math.Round(clampedHealth / (double)maxHealthValue * 100);
      int filled = (int)Math.Round(percent / 100.0 * width);
      int empty = width - filled;

      string fill = Markup.Escape(new string('#', filled));
      string rest = Markup.Escape(new string('-', empty));
      var level = Health.EvaluateHealthLevel(percent);
      string color = GetHealthColor(level);

      return $"Health [{color}]{fill}[/][grey]{rest}[/][grey] {percent,3}%[/]";
   }
}
