using GameEngine;
using Moutons.Core;
using static Moutons.UI.ConsoleUtils;
using Spectre.Console;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using DrawingColor = System.Drawing.Color;

AnsiConsole.Console.Profile.Capabilities.Unicode = false;

const int maxHealth = 100;
const int healthBarWidth = 24;
const int canvasWidth = 102;
const int canvasHeight = 42;

// TODO Add more unit tests if needed.
GameLogic gameLogic = new GameLogic(canvasWidth, canvasHeight, new Point(1, 1));
GameLogic lastDisplayedGameLogic = gameLogic;
GameRenderer renderer = new GameRenderer(canvasWidth, canvasHeight);
Dictionary<Point, DrawingColor> pixels = new Dictionary<Point, DrawingColor>(renderer.Buffer.Render());
Stopwatch timer = Stopwatch.StartNew();
int frameCount = 0;
long startElapsedMilliseconds = 0;
int health = 100;
long lastHealthDecreaseMilliseconds = 0;
bool renderGame = true;
bool updateDisplay = true;

Layout layout = new Layout("Root")
   .SplitRows(
      new Layout("Top").Size(1),
      new Layout("Bottom").Size(canvasHeight));

   layout["Top"].Update(Align.Center(new Markup("[grey]Starting...[/]")));
layout["Bottom"].Update(new Markup(BuildCanvasMarkup(canvasWidth, canvasHeight, pixels)));

AnsiConsole.Live(layout)
   .AutoClear(false)
   .Start(ctx =>
   {
      // TODO Restore logic to adapt the UI to the console size.
      while (true)
      {
         frameCount++;
         long elapsedMilliseconds = timer.ElapsedMilliseconds;
         long timeSinceLastFPSUpdate = elapsedMilliseconds - startElapsedMilliseconds;

         if (timeSinceLastFPSUpdate > 250)
         {
            int fps = (int)Math.Round(frameCount / (timeSinceLastFPSUpdate / 1000.0));
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

         if (renderGame)
         {
            foreach (KeyValuePair<Point, DrawingColor> changedPixel in renderer.Render(lastDisplayedGameLogic, gameLogic))
            {
               pixels[changedPixel.Key] = changedPixel.Value;
            }

            layout["Bottom"].Update(new Markup(BuildCanvasMarkup(canvasWidth, canvasHeight, pixels)));
            lastDisplayedGameLogic = gameLogic;
            renderGame = false;
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

            if (!key.HasValue)
            {
               continue;
            }

            if (key.Value.Key == ConsoleKey.Escape)
            {
               return;
            }

            gameLogic = key.Value.Key switch
            {
               ConsoleKey.LeftArrow => gameLogic.MoveSheep(Direction.Left),
               ConsoleKey.RightArrow => gameLogic.MoveSheep(Direction.Right),
               ConsoleKey.UpArrow => gameLogic.MoveSheep(Direction.Up),
               ConsoleKey.DownArrow => gameLogic.MoveSheep(Direction.Down),
               _ => gameLogic
            };

            if (gameLogic.StateChanged)
            {
               renderGame = true;
            }
         }
      }
   });
