using Spectre.Console;

var canvas = new Canvas(100, 48);

canvas.Scale = false;
canvas.PixelWidth = 2;

// UNDONE Add a status bar
AnsiConsole.Live(canvas)
    .Start(ctx =>
    {
       System.Drawing.Point sheepPosition = new System.Drawing.Point(1, 1);
       System.Drawing.Point previousSheepPosition = sheepPosition;

       while (true)
       {
          if (AnsiConsole.Console.Profile.Width >= canvas.Width && AnsiConsole.Console.Profile.Height >= canvas.Height)
          {
             // Fill background
             // UNDONE Move this to a library outside the UI
             for (var x = 0; x < canvas.Width; x++)
             {
                canvas.SetPixel(x, 0, Color.Gray19);
                canvas.SetPixel(x, canvas.Height - 1, Color.Gray19);
             }

             for (var y = 1; y < canvas.Height; y++)
             {
                canvas.SetPixel(0, y, Color.Gray19);
                canvas.SetPixel(canvas.Width - 1, y, Color.Gray19);
             }

             canvas.SetPixel(previousSheepPosition.X, previousSheepPosition.Y, Color.Black);
             canvas.SetPixel(sheepPosition.X, sheepPosition.Y, Color.White);
          }
          else
          {
             // clear screen
             // UNDONE
          }

          ctx.Refresh();
          // UNDONE Remove this sleep
          //Thread.Sleep(1000);

          if (AnsiConsole.Console.Input.IsKeyAvailable())
          {
             ConsoleKeyInfo? key = AnsiConsole.Console.Input.ReadKey(true);

             if (key?.Key == ConsoleKey.Escape)
             {
                return;
             }
             else
             {
                switch(key?.Key)
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