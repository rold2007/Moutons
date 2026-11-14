using GameEngine;
using Moutons.Core;
using Spectre.Console;
using System.Drawing;
using System.Diagnostics;
using DrawingColor = System.Drawing.Color;

namespace Moutons.UI;

public class GameApp
{
   private readonly int canvasWidth;
   private readonly int canvasHeight;
   private readonly int maxHealth;
   private readonly int healthBarWidth;

   private GameLogic gameLogic;
   private GameLogic lastDisplayedGameLogic;
   private GameRenderer renderer;
   private Dictionary<Point, DrawingColor> pixels;
   private Stopwatch timer;
   private int frameCount;
   private long startElapsedMilliseconds;
   private int health;
   private long lastHealthDecreaseMilliseconds;
   private bool renderGame;
   private bool updateDisplay;

   public GameApp(int canvasWidth, int canvasHeight, int maxHealth, int healthBarWidth)
   {
      this.canvasWidth = canvasWidth;
      this.canvasHeight = canvasHeight;
      this.maxHealth = maxHealth;
      this.healthBarWidth = healthBarWidth;

      gameLogic = new GameLogic(canvasWidth, canvasHeight);
      lastDisplayedGameLogic = gameLogic;
      renderer = new GameRenderer(canvasWidth, canvasHeight);

      // TODO This is a temporary solution to initialize the pixels dictionary with the initial state of the game. Implement a better fix.
      pixels = new Dictionary<Point, DrawingColor>(renderer.Buffer.Render());

      gameLogic = gameLogic.SpawnSheep(new Point(1, 1));
      gameLogic = gameLogic.SpawnGrass(new Point(10, 10));
      gameLogic = gameLogic.SpawnGrass(new Point(10, 11));
      gameLogic = gameLogic.SpawnGrass(new Point(10, 12));
      gameLogic = gameLogic.SpawnGrass(new Point(11, 10));
      gameLogic = gameLogic.SpawnGrass(new Point(11, 11));
      gameLogic = gameLogic.SpawnGrass(new Point(11, 12));
      gameLogic = gameLogic.SpawnGrass(new Point(12, 10));
      gameLogic = gameLogic.SpawnGrass(new Point(12, 11));
      gameLogic = gameLogic.SpawnGrass(new Point(12, 12));
      gameLogic = gameLogic.SpawnGrass(new Point(13, 11));

      timer = Stopwatch.StartNew();
      frameCount = 0;
      startElapsedMilliseconds = 0;
      health = maxHealth;
      lastHealthDecreaseMilliseconds = 0;
      renderGame = true;
      updateDisplay = true;
   }

   public void Run(Layout layout, LiveDisplayContext ctx)
   {
      layout["Top"].Update(Align.Center(new Markup("[grey]Starting...[/]")));
      layout["Bottom"].Update(new Markup(ConsoleUtils.BuildCanvasMarkup(canvasWidth, canvasHeight, pixels)));

      while (true)
      {
         frameCount++;
         long elapsedMilliseconds = timer.ElapsedMilliseconds;
         long timeSinceLastFPSUpdate = elapsedMilliseconds - startElapsedMilliseconds;

         if (timeSinceLastFPSUpdate > 250)
         {
            UpdateFpsDisplay(layout, elapsedMilliseconds);
         }

         long timeSinceLastHealthUpdate = elapsedMilliseconds - lastHealthDecreaseMilliseconds;

         if (timeSinceLastHealthUpdate >= 3000)
         {
            UpdateHealth();
         }

         if (renderGame)
         {
            RenderGameFrame(layout);
         }

         if (updateDisplay)
         {
            ctx.Refresh();
            updateDisplay = false;
         }

         HandlePlayerInput();
      }
   }

   private void UpdateFpsDisplay(Layout layout, long elapsedMilliseconds)
   {
      int fps = (int)Math.Round(frameCount / (elapsedMilliseconds / 1000.0));
      string healthBar = ConsoleUtils.BuildHealthBarMarkup(health, maxHealth, healthBarWidth);
      layout["Top"].Update(Align.Center(new Markup($"{healthBar} [grey]|[/] {fps} FPS")));
      frameCount = 0;
      startElapsedMilliseconds = elapsedMilliseconds;
      updateDisplay = true;
   }

   private void UpdateHealth()
   {
      health = Math.Max(0, health - 1);
      lastHealthDecreaseMilliseconds = timer.ElapsedMilliseconds;
   }

   private void RenderGameFrame(Layout layout)
   {
      foreach (KeyValuePair<Point, DrawingColor> changedPixel in renderer.Render(lastDisplayedGameLogic, gameLogic))
      {
         pixels[changedPixel.Key] = changedPixel.Value;
      }

      layout["Bottom"].Update(new Markup(ConsoleUtils.BuildCanvasMarkup(canvasWidth, canvasHeight, pixels)));
      lastDisplayedGameLogic = gameLogic;
      renderGame = false;
      updateDisplay = true;
   }

   private void HandlePlayerInput()
   {
      if (AnsiConsole.Console.Input.IsKeyAvailable())
      {
         ConsoleKeyInfo? key = AnsiConsole.Console.Input.ReadKey(true);

         if (!key.HasValue)
         {
            return;
         }

         if (key.Value.Key == ConsoleKey.Escape)
         {
            Environment.Exit(0);
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
}
