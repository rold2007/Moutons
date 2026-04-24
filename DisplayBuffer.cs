using Spectre.Console;

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
}