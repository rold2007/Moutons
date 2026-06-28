using System.Collections.Immutable;
using System.Drawing;

namespace GameEngine
{
   public class DisplayBuffer
   {
      private ImmutableDictionary<Point, Color> ChangedPixels { get; set; } = ImmutableDictionary<Point, Color>.Empty;
      public int Width { get; }
      public int Height { get; }

      private DisplayBuffer(int width, int height, ImmutableDictionary<Point, Color> changedPixels) : this(width, height)
      {
         ChangedPixels = changedPixels;
      }

      public DisplayBuffer(int width, int height)
      {
         Width = width;
         Height = height;
      }

      public DisplayBuffer SetPixel(Point point, Color color)
      {
         return new DisplayBuffer(Width, Height, ChangedPixels.SetItem(point, color));
      }

      public DisplayBuffer Clear(Color clearColor)
      {
         ImmutableDictionary<Point, Color> changedPixels = ImmutableDictionary<Point, Color>.Empty;

         for (int x = 0; x < Width; x++)
         {
            for (int y = 0; y < Height; y++)
            {
               changedPixels = changedPixels.Add(new Point(x, y), clearColor);
            }
         }

         return new DisplayBuffer(Width, Height, changedPixels);
      }

      public ImmutableDictionary<Point, Color> Render()
      {
         return ChangedPixels;
      }
   }
}