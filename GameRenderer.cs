using Spectre.Console;

namespace GameEngine
{
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