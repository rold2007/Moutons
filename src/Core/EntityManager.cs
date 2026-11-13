using System.Drawing;

namespace Moutons.Core;

public class EntityManager
{
   // TODO Add many sheeps
   public Point SheepPosition { get; }

   // TODO Add grass resources

   public EntityManager(Point sheepPosition)
   {
      SheepPosition = sheepPosition;
   }
}
