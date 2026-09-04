using System;
using Spectre.Console;

namespace GameConsole;

public static class ConsoleUtils
{
   public static string GetHealthColor(int healthPercent)
   {
      // TODO The colors should be defined by the game.
      return healthPercent switch
      {
         >= 67 => "green",
         >= 34 => "yellow",
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
      string color = GetHealthColor(percent);

      return $"Health [{color}]{fill}[/][grey]{rest}[/][grey] {percent,3}%[/]";
   }
}
