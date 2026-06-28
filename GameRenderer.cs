using System.Collections.Immutable;
using System.Drawing;

namespace GameEngine
{
   public readonly struct PixelChange
   {
      public int X { get; }
      public int Y { get; }
      public Color Color { get; }

      public PixelChange(int x, int y, Color color)
      {
         X = x;
         Y = y;
         Color = color;
      }
   }

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
         for (var x = 0; x < targetBuffer.Width; x++)
         {
            targetBuffer = targetBuffer.SetPixel(new System.Drawing.Point(x, 0), borderColor);
            targetBuffer = targetBuffer.SetPixel(new System.Drawing.Point(x, targetBuffer.Height - 1), borderColor);
         }

         for (var y = 1; y < targetBuffer.Height; y++)
         {
            targetBuffer = targetBuffer.SetPixel(new System.Drawing.Point(0, y), borderColor);
            targetBuffer = targetBuffer.SetPixel(new System.Drawing.Point(targetBuffer.Width - 1, y), borderColor);
         }

         return targetBuffer;
      }

      public ImmutableDictionary<System.Drawing.Point, Color> Render(System.Drawing.Point sheepPosition, System.Drawing.Point previousSheepPosition)
      {
         Buffer = Buffer.SetPixel(new System.Drawing.Point(previousSheepPosition.X, previousSheepPosition.Y), Color.Black);
         Buffer = Buffer.SetPixel(new System.Drawing.Point(sheepPosition.X, sheepPosition.Y), Color.White);

         ImmutableDictionary<System.Drawing.Point, Color> changedPixels = Buffer.Render();

         Buffer = new DisplayBuffer(Buffer.Width, Buffer.Height);

         return changedPixels;
      }
   }
}