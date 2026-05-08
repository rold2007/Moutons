using Spectre.Console;

namespace GameEngine
{
   public class DisplayBuffer
   {
      // TODO No need to save the color. Just use the current buffer with the pixel position to fetch the coor to apply.
      private Color[,] buffer;
      public int Width { get; }
      public int Height { get; }

      public DisplayBuffer(int width, int height)
      {
         Width = width;
         Height = height;
         buffer = new Color[width, height];
      }

      public List<PixelChange> SetPixel(int x, int y, Color color)
      {
         if (buffer[x, y] == color)
         {
            return new List<PixelChange>();
         }
         else
         {
            buffer[x, y] = color;
            return new List<PixelChange> { new PixelChange(x, y, color) };
         }
      }

      public Color GetPixel(int x, int y)
      {
         return buffer[x, y];
      }

      public List<PixelChange> Clear(Color clearColor)
      {
         List<PixelChange> changedPixels = new List<PixelChange>();

         for (var x = 0; x < Width; x++)
         {
            for (var y = 0; y < Height; y++)
            {
               SetPixel(x, y, clearColor);
               changedPixels.Add(new PixelChange(x, y, clearColor));
            }
         }

         return changedPixels;
      }

      public List<PixelChange> CopyFrom(DisplayBuffer other)
      {
         List<PixelChange> changedPixels = new List<PixelChange>();

         for (int x = 0; x < Width; x++)
         {
            for (int y = 0; y < Height; y++)
            {
               Color otherColor = other.buffer[x, y];

               if (buffer[x, y] != otherColor)
               {
                  buffer[x, y] = otherColor;
                  changedPixels.Add(new PixelChange(x, y, otherColor));
               }
            }
         }

         return changedPixels;
      }
   }
}