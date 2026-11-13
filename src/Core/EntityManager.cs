using System.Collections.Immutable;
using System.Drawing;

namespace Moutons.Core;

public class EntityManager
{
   // TODO Add many sheeps
   public Point SheepPosition { get; }
   public ImmutableArray<Point> GrassPositions { get; }

   public EntityManager(Point sheepPosition, ImmutableArray<Point> grassPositions = default)
   {
      SheepPosition = sheepPosition;
      GrassPositions = grassPositions.IsDefault ? ImmutableArray<Point>.Empty : grassPositions;
   }
}
