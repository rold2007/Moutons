using Spectre.Console;
using Moutons;
using System.Drawing;
using System.Text;
using DrawingColor = System.Drawing.Color;

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

   public static string BuildCanvasMarkup(int width, int height, IReadOnlyDictionary<Point, DrawingColor> pixels)
   {
      StringBuilder markup = new StringBuilder();

      for (int y = 0; y < height; y++)
      {
         for (int x = 0; x < width; x++)
         {
            DrawingColor color = pixels[new Point(x, y)];
            markup.Append($"[on rgb({color.R},{color.G},{color.B})]  [/]");
         }

         if (y < height - 1)
         {
            markup.AppendLine();
         }
      }

      return markup.ToString();
   }
}
