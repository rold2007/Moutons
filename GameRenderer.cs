using Spectre.Console;
using System.Collections.Generic;

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
      private DisplayBuffer[] buffers;
      private int currentBufferIndex = 0;
      public DisplayBuffer Buffer => buffers[currentBufferIndex];

      public GameRenderer(int width, int height)
      {
         buffers = new DisplayBuffer[2];
         buffers[0] = new DisplayBuffer(width, height);
         buffers[1] = new DisplayBuffer(width, height);

         buffers[0].Clear(Color.Black);
         buffers[1].Clear(Color.White);

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

      public List<PixelChange> Render(System.Drawing.Point sheepPosition, System.Drawing.Point previousSheepPosition)
      {
         int nextBufferIndex = 1 - currentBufferIndex;
         List<PixelChange> changedPixels = new List<PixelChange>();

         changedPixels.AddRange(buffers[nextBufferIndex].CopyFrom(buffers[currentBufferIndex]));
         changedPixels.AddRange(buffers[nextBufferIndex].SetPixel(previousSheepPosition.X, previousSheepPosition.Y, Color.Black));
         changedPixels.AddRange(buffers[nextBufferIndex].SetPixel(sheepPosition.X, sheepPosition.Y, Color.White));

         currentBufferIndex = nextBufferIndex;

         return changedPixels;
      }
   }
}