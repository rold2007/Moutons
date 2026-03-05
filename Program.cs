using Spectre.Console;

var canvas = new Canvas(40, 20);

canvas.Scale = false;

AnsiConsole.Live(canvas)
    .Start(ctx =>
    {
        int count = 0;

        while (true)
        {
            // Fill background
            for (var y = 0; y < 20; y++)
            {
                for (var x = 0; x < 40; x++)
                {
                    // canvas.SetPixel(x, y, Color.Grey11);
                    if (count % 2 == 0)
                        canvas.SetPixel(x, y, Color.Red);
                    else
                        canvas.SetPixel(x, y, Color.Yellow);
                }
            }

            ctx.Refresh();
            Thread.Sleep(1000);
            count++;
        }
    });