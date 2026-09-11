using System.Drawing;

namespace Moutons.Core;

public class EntityManager
{
    public Point SheepPosition { get; }

    public EntityManager(Point sheepPosition)
    {
        SheepPosition = sheepPosition;
    }
}
