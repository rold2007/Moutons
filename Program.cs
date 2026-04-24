using GameEngine;
using Spectre.Console;
using System.Diagnostics;
using System.Drawing;

AnsiConsole.Console.Profile.Capabilities.Unicode = false;

Canvas canvas = new Canvas(100, 48);
Text statusbar = new Text("Use arrow keys to move the sheep. Press ESC to exit.")
    .Centered();

canvas.Scale = false;

Layout layout = new Layout("Root")
    .SplitRows(
        new Layout("Top").Size(1),
        new Layout("Bottom"));

layout["Top"].Update(statusbar);
layout["Bottom"].Update(canvas);

AnsiConsole.Live(layout)
    .Start(ctx =>
    {
       Point sheepPosition = new Point(1, 1);
       Point previousSheepPosition = sheepPosition;
       bool restorelayout = false;
       Stopwatch timer = Stopwatch.StartNew();
       int frameCount = 0;
       long startElapsedMilliseconds = 0;
       int health = 100;
       long lastHealthDecreaseMilliseconds = 0;
       GameRenderer renderer = new GameRenderer(canvas.Width, canvas.Height);

       // HACK Using a text cursor indicator (Settings->Accessibility->Text cursor) makes the cursor visible and moving at each refresh. Find a way to hide the cursor. There is no easy way. Keep it as is for now.
       while (true)
       {
          bool updateDisplay = false;

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

                // TODO Replace health text to progress bar with colors (red, yellow, green))
                layout["Top"].Update(new Text($"Health: {health} | {fps} FPS").Centered());

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

             if (sheepPosition != previousSheepPosition)
             {
                // TODO Add more entities and only update the pixels that changed instead of redrawing the entire canvas every frame
                // UNDONE Simplify this logic to only update the pixels that changed instead of redrawing the entire canvas every frame
                renderer.Render(sheepPosition, previousSheepPosition);
                previousSheepPosition = sheepPosition;

                // Copy display buffer to canvas
                for (var x = 0; x < canvas.Width; x++)
                {
                   for (var y = 0; y < canvas.Height; y++)
                   {
                      canvas.SetPixel(x, y, renderer.Buffer.GetPixel(x, y));
                   }
                }

                updateDisplay = true;
             }
          }
          else
          {
             // TODO Detect if the game just started and pause the game loop until the window is resized to the correct size
             ctx.UpdateTarget(new Text("Console window is too small. Maximize the window."));
             restorelayout = true;
             updateDisplay = true;
          }

          if (updateDisplay)
          {
             ctx.Refresh();
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
             }
          }
       }
    });