using Moutons.UI;
using Spectre.Console;

AnsiConsole.Console.Profile.Capabilities.Unicode = false;

const int maxHealth = 100;
const int healthBarWidth = 24;
const int canvasWidth = 102;
const int canvasHeight = 42;

// TODO Restore logic to adapt the UI to the console size.

Layout layout = new Layout("Root")
   .SplitRows(
      new Layout("Top").Size(1),
      new Layout("Bottom").Size(canvasHeight));

GameApp gameApp = new GameApp(canvasWidth, canvasHeight, maxHealth, healthBarWidth);

AnsiConsole.Live(layout)
   .AutoClear(false)
   .Start(ctx =>
   {
      gameApp.Run(layout, ctx);
   });
