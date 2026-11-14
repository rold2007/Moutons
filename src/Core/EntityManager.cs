using System.Collections.Immutable;
using System.Drawing;

namespace Moutons.Core;

public class EntityManager
{
    // TODO Add many sheeps
    public ImmutableHashSet<Point> SheepPositions { get; }
    public ImmutableHashSet<Point> GrassPositions { get; }

    public EntityManager(ImmutableHashSet<Point> sheepPositions)
        : this(sheepPositions, ImmutableHashSet<Point>.Empty)
    {
    }

    public EntityManager(ImmutableHashSet<Point> sheepPositions, ImmutableHashSet<Point> grassPositions)
    {
        SheepPositions = sheepPositions;
        GrassPositions = grassPositions;
    }
}
