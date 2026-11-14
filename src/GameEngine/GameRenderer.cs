using System.Collections.Immutable;
using System.Drawing;
using Moutons.Core;

namespace GameEngine;

public class GameRenderer
{
   public DisplayBuffer Buffer { get; private set; }

   public GameRenderer(int width, int height)
   {
      Buffer = new DisplayBuffer(width, height);

      Buffer = Buffer.Clear(Color.Black);

      Buffer = DrawBorders(Buffer, Color.FromArgb(48, 48, 48));
   }

   public DisplayBuffer DrawBorders(DisplayBuffer targetBuffer, Color borderColor)
   {
      for (int x = 0; x < targetBuffer.Width; x++)
      {
         targetBuffer = targetBuffer.SetPixel(new System.Drawing.Point(x, 0), borderColor);
         targetBuffer = targetBuffer.SetPixel(new System.Drawing.Point(x, targetBuffer.Height - 1), borderColor);
      }

      for (int y = 1; y < targetBuffer.Height; y++)
      {
         targetBuffer = targetBuffer.SetPixel(new System.Drawing.Point(0, y), borderColor);
         targetBuffer = targetBuffer.SetPixel(new System.Drawing.Point(targetBuffer.Width - 1, y), borderColor);
      }

      return targetBuffer;
   }

   public ImmutableDictionary<System.Drawing.Point, Color> Render(GameLogic previousGameLogic, GameLogic currentGameLogic)
   {
      ImmutableHashSet<Point> newSheepPositions = currentGameLogic.SheepPositions.Except(previousGameLogic.SheepPositions);
      ImmutableHashSet<Point> newGrassPositions = currentGameLogic.GrassPositions.Except(previousGameLogic.GrassPositions);

      newGrassPositions = newGrassPositions.Except(currentGameLogic.SheepPositions);

      if (!newSheepPositions.IsEmpty || !newGrassPositions.IsEmpty)
      {
         ImmutableHashSet<Point> oldSheepPositions = previousGameLogic.SheepPositions.Except(currentGameLogic.SheepPositions);

         foreach (Point oldSheepPosition in oldSheepPositions)
         {
            if (currentGameLogic.GrassPositions.Contains(oldSheepPosition))
            {
               Buffer = Buffer.SetPixel(oldSheepPosition, Color.Green);
            }
            else
            {
               Buffer = Buffer.SetPixel(oldSheepPosition, Color.Black);
            }
         }

         foreach (Point newGrassPosition in newGrassPositions)
         {
            Buffer = Buffer.SetPixel(newGrassPosition, Color.Green);
         }

         foreach (Point newSheepPosition in newSheepPositions)
         {
            Buffer = Buffer.SetPixel(newSheepPosition, Color.White);
         }

         ImmutableDictionary<System.Drawing.Point, Color> changedPixels = Buffer.Render();

         Buffer = Buffer.Reset();

         return changedPixels;
      }

      return [];
   }
}