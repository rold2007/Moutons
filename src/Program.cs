using GameEngine;
using Moutons;
using static GameConsole.ConsoleUtils;
using Spectre.Console;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;

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

layout["Top"].Update(statusbar);
layout["Bottom"].Update(canvas);

AnsiConsole.Live(layout)
    .Start(ctx =>
    {
       int worldWidth = canvas.Width;
       int worldHeight = canvas.Height;
       GameLogic gameLogic = new GameLogic(worldWidth, worldHeight, new Point(1, 1));
       bool restorelayout = false;
       Stopwatch timer = Stopwatch.StartNew();
       int frameCount = 0;
       long startElapsedMilliseconds = 0;
       int health = 100;
       long lastHealthDecreaseMilliseconds = 0;
       GameRenderer renderer = new GameRenderer(canvas.Width, canvas.Height);
       bool updateDisplay = true;
       bool drawSheep = true;
       bool uiActive = true;
       int consoleWidthError = 0;
       int consoleHeightError = 0;

       // HACK Using a text cursor indicator (Settings->Accessibility->Text cursor) makes the cursor visible and moving at each refresh. Find a way to hide the cursor. There is no easy way. Keep it as is for now.
       while (true)
       {
          if (AnsiConsole.Console.Profile.Width >= canvas.Width * 2 && AnsiConsole.Console.Profile.Height >= canvas.Height + 1)
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
                ImmutableDictionary<System.Drawing.Point, System.Drawing.Color> changedPixels = renderer.Render(gameLogic.SheepPosition, gameLogic.PreviousSheepPosition);

                foreach (KeyValuePair<Point, System.Drawing.Color> kvp in changedPixels)
                {
                   canvas.SetPixel(kvp.Key.X, kvp.Key.Y, new Spectre.Console.Color(kvp.Value.R, kvp.Value.G, kvp.Value.B));
                }

                updateDisplay = true;
                drawSheep = false;
             }

             consoleWidthError = 0;
             consoleHeightError = 0;
             uiActive = true;
          }
          else
          {
             if (consoleWidthError != AnsiConsole.Console.Profile.Width || consoleHeightError != AnsiConsole.Console.Profile.Height)
             {
                consoleWidthError = AnsiConsole.Console.Profile.Width;
                consoleHeightError = AnsiConsole.Console.Profile.Height;

                AnsiConsole.Console.Clear();

                string errorMessage = string.Format("Console window is too small. Current size: {0}x{1}. Required size: {2}x{3}. Maximize the window.", AnsiConsole.Console.Profile.Width, AnsiConsole.Console.Profile.Height, canvas.Width * 2, canvas.Height + 1);

                ctx.UpdateTarget(new Text(errorMessage));
                restorelayout = true;
                updateDisplay = true;
                uiActive = false;
             }
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
                if (uiActive)
                {
                   switch (key?.Key)
                   {
                      case ConsoleKey.LeftArrow:
                         gameLogic = gameLogic.MoveSheep(Direction.Left);
                         break;
                      case ConsoleKey.RightArrow:
                         gameLogic = gameLogic.MoveSheep(Direction.Right);
                         break;
                      case ConsoleKey.UpArrow:
                         gameLogic = gameLogic.MoveSheep(Direction.Up);
                         break;
                      case ConsoleKey.DownArrow:
                         gameLogic = gameLogic.MoveSheep(Direction.Down);
                         break;
                   }

                   if (gameLogic.SheepPositionChanged)
                   {
                      drawSheep = true;
                   }
                }
             }
          }
       }
    });
