using GameEngine;
using Spectre.Console;
using System.Diagnostics;

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
       System.Drawing.Point sheepPosition = new System.Drawing.Point(1, 1);
       System.Drawing.Point previousSheepPosition = sheepPosition;
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
                   // UNDONE Make sure all loops are optimized. I think it is faster with the Y in the outer loop.
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

namespace GameEngine
{
   public class DisplayBuffer
   {
      private Color[,] buffer;
      public int Width { get; }
      public int Height { get; }

      public DisplayBuffer(int width, int height)
      {
         Width = width;
         Height = height;
         buffer = new Color[width, height];
      }

      public void SetPixel(int x, int y, Color color)
      {
         buffer[x, y] = color;
      }

      public Color GetPixel(int x, int y)
      {
         return buffer[x, y];
      }

      public void Clear(Color clearColor)
      {
         for (var x = 0; x < Width; x++)
         {
            for (var y = 0; y < Height; y++)
            {
               SetPixel(x, y, clearColor);
            }
         }
      }

      public void CopyFrom(DisplayBuffer other)
      {
         for (int x = 0; x < Width; x++)
         {
            for (int y = 0; y < Height; y++)
            {
               buffer[x, y] = other.buffer[x, y];
            }
         }
      }
   }

   public class GameRenderer
   {
      private DisplayBuffer[] buffers;
      private int currentBufferIndex = 0;
      public DisplayBuffer Buffer => buffers[currentBufferIndex];

      public GameRenderer(int width, int height)
      {
         buffers = new DisplayBuffer[2];
         buffers[0] = new DisplayBuffer(width, height);
         buffers[1] = new DisplayBuffer(width, height);

         buffers[0].Clear(Color.Black);
         buffers[1].Clear(Color.Black);

         DrawBorders(buffers[0], Color.Gray19);
      }

      public void DrawBorders(DisplayBuffer targetBuffer, Color borderColor)
      {
         for (var x = 0; x < targetBuffer.Width; x++)
         {
            targetBuffer.SetPixel(x, 0, borderColor);
            targetBuffer.SetPixel(x, targetBuffer.Height - 1, borderColor);
         }

         for (var y = 1; y < targetBuffer.Height; y++)
         {
            targetBuffer.SetPixel(0, y, borderColor);
            targetBuffer.SetPixel(targetBuffer.Width - 1, y, borderColor);
         }
      }

      public void Render(System.Drawing.Point sheepPosition, System.Drawing.Point previousSheepPosition)
      {
         int nextBufferIndex = 1 - currentBufferIndex;

         buffers[nextBufferIndex].CopyFrom(buffers[currentBufferIndex]);
         buffers[nextBufferIndex].SetPixel(previousSheepPosition.X, previousSheepPosition.Y, Color.Black);
         buffers[nextBufferIndex].SetPixel(sheepPosition.X, sheepPosition.Y, Color.White);

         currentBufferIndex = nextBufferIndex;
      }
   }
}