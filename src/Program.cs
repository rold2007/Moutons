using GameEngine;
using Spectre.Console;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;

// UNDONE Add EntityManager and GameLogic classes.
// UNDONE GameLogic may depend on EntityManager, but EntityManager must never depend on GameLogic.  
// UNDONE GameRenderer must depend on neither.

AnsiConsole.Console.Profile.Capabilities.Unicode = false;

Canvas canvas = new Canvas(102, 42);
Text statusbar = new Text("Use arrow keys to move the sheep. Press ESC to exit.")
    .Centered();

canvas.Scale = false;

Layout layout = new Layout("Root")
    .SplitRows(
        new Layout("Top").Size(1),
        new Layout("Bottom"));

const int maxHealth = 100;
const int healthBarWidth = 24;

// TODO Move this to a separate non-UI class and add unit tests
static string GetHealthColor(int healthPercent)
{
   return healthPercent switch
   {
      >= 67 => "green",
      >= 34 => "yellow",
      _ => "red"
   };
}

// TODO Move most of this to a separate non-UI class and add unit tests
static string BuildHealthBarMarkup(int health, int maxHealthValue, int width)
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

layout["Top"].Update(statusbar);
layout["Bottom"].Update(canvas);

AnsiConsole.Live(layout)
    .Start(ctx =>
    {
       Point sheepPosition = new Point(1, 1);
       Point previousSheepPosition = new Point(1, 1);
       bool restorelayout = false;
       Stopwatch timer = Stopwatch.StartNew();
       int frameCount = 0;
       long startElapsedMilliseconds = 0;
       int health = 100;
       long lastHealthDecreaseMilliseconds = 0;
       GameRenderer renderer = new GameRenderer(canvas.Width, canvas.Height);
       bool updateDisplay = true;
       bool drawSheep = true;

       // HACK Using a text cursor indicator (Settings->Accessibility->Text cursor) makes the cursor visible and moving at each refresh. Find a way to hide the cursor. There is no easy way. Keep it as is for now.
       while (true)
       {
          if (AnsiConsole.Console.Profile.Width >= canvas.Width && AnsiConsole.Console.Profile.Height >= canvas.Height)
          {
             if (restorelayout)
             {
                ctx.UpdateTarget(layout);
                restorelayout = false;
                updateDisplay = true;
             }

             frameCount++;

             long elapsedMilliseconds = timer.ElapsedMilliseconds;
             long timeSinceLastFPSUpdate = elapsedMilliseconds - startElapsedMilliseconds;

             if (timeSinceLastFPSUpdate > 100)
             {
                int fps = (int)Math.Round(frameCount / (elapsedMilliseconds / 1000.0));

                string healthBar = BuildHealthBarMarkup(health, maxHealth, healthBarWidth);
                layout["Top"].Update(Align.Center(new Markup($"{healthBar} [grey]|[/] {fps} FPS")));

                frameCount = 0;
                startElapsedMilliseconds = elapsedMilliseconds;
                updateDisplay = true;
             }

             long timeSinceLastHealthUpdate = elapsedMilliseconds - lastHealthDecreaseMilliseconds;

             if (timeSinceLastHealthUpdate >= 3000)
             {
                health = Math.Max(0, health - 1);
                lastHealthDecreaseMilliseconds = elapsedMilliseconds;
             }

             if (drawSheep)
             {
                // TODO Add more entities and only update the pixels that changed instead of redrawing the entire canvas every frame
                ImmutableDictionary<System.Drawing.Point, System.Drawing.Color> changedPixels = renderer.Render(sheepPosition, previousSheepPosition);
                previousSheepPosition = sheepPosition;

                foreach (KeyValuePair<Point, System.Drawing.Color> kvp in changedPixels)
                {
                   canvas.SetPixel(kvp.Key.X, kvp.Key.Y, new Spectre.Console.Color(kvp.Value.R, kvp.Value.G, kvp.Value.B));
                }

                updateDisplay = true;
                drawSheep = false;
             }
          }
          else
          {
             AnsiConsole.Console.Clear();

             // TODO Detect if the game just started and pause the game loop until the window is resized to the correct size
             string errorMessage = string.Format("Console window is too small. Current size: {0}x{1}. Required size: {2}x{3}. Maximize the window.", AnsiConsole.Console.Profile.Width, AnsiConsole.Console.Profile.Height, canvas.Width * 2, canvas.Height);

             ctx.UpdateTarget(new Text(errorMessage));
             restorelayout = true;
             updateDisplay = true;
          }

          if (updateDisplay)
          {
             ctx.Refresh();
             updateDisplay = false;
          }

          if (AnsiConsole.Console.Input.IsKeyAvailable())
          {
             ConsoleKeyInfo? key = AnsiConsole.Console.Input.ReadKey(true);

             if (key?.Key == ConsoleKey.Escape)
             {
                return;
             }
             else
             {
                switch (key?.Key)
                {
                   case ConsoleKey.LeftArrow:
                      previousSheepPosition = sheepPosition;
                      sheepPosition.X = Math.Max(1, sheepPosition.X - 1);
                      break;
                   case ConsoleKey.RightArrow:
                      previousSheepPosition = sheepPosition;
                      sheepPosition.X = Math.Min(canvas.Width - 2, sheepPosition.X + 1);
                      break;
                   case ConsoleKey.UpArrow:
                      previousSheepPosition = sheepPosition;
                      sheepPosition.Y = Math.Max(1, sheepPosition.Y - 1);
                      break;
                   case ConsoleKey.DownArrow:
                      previousSheepPosition = sheepPosition;
                      sheepPosition.Y = Math.Min(canvas.Height - 2, sheepPosition.Y + 1);
                      break;
                }

                if (sheepPosition != previousSheepPosition)
                {
                   drawSheep = true;
                }
             }
          }
       }
    });
