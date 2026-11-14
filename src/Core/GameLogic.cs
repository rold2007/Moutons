using System.Collections.Immutable;
using System.Drawing;

namespace Moutons.Core;

public enum Direction
{
   Left,
   Right,
   Up,
   Down
}

public class GameLogic
{
   private readonly int worldWidth;
   private readonly int worldHeight;

   private EntityManager CurrentState { get; }
   private EntityManager PreviousState { get; }

   public ImmutableHashSet<Point> SheepPositions => CurrentState.SheepPositions;
   public ImmutableHashSet<Point> PreviousSheepPositions => PreviousState.SheepPositions;
   public ImmutableHashSet<Point> GrassPositions => CurrentState.GrassPositions;
   public bool SheepPositionChanged => !SheepPositions.SetEquals(PreviousSheepPositions);
   public bool StateChanged => SheepPositionChanged;

   public GameLogic(int worldWidth, int worldHeight)
   {
      this.worldWidth = worldWidth;
      this.worldHeight = worldHeight;
      CurrentState = new EntityManager([], []);
      PreviousState = new EntityManager([], []);
   }

   private GameLogic(int worldWidth, int worldHeight, EntityManager currentState, EntityManager previousState)
   {
      this.worldWidth = worldWidth;
      this.worldHeight = worldHeight;
      CurrentState = currentState;
      PreviousState = previousState;
   }

   public GameLogic MoveSheep(Direction direction)
   {
      Point delta = direction switch
      {
         Direction.Left => new Point(-1, 0),
         Direction.Right => new Point(1, 0),
         Direction.Up => new Point(0, -1),
         Direction.Down => new Point(0, 1),
         _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unsupported direction.")
      };

      ImmutableHashSet<Point> newSheepPositions = CurrentState.SheepPositions
         .Select(position => new Point(
            Math.Clamp(position.X + delta.X, 1, worldWidth - 2),
            Math.Clamp(position.Y + delta.Y, 1, worldHeight - 2)))
         .ToImmutableHashSet();
      return new GameLogic(worldWidth, worldHeight, new EntityManager(newSheepPositions, CurrentState.GrassPositions), CurrentState);
   }

   public GameLogic SpawnGrass(Point grassPosition)
   {
      return new GameLogic(worldWidth, worldHeight, new EntityManager(CurrentState.SheepPositions, CurrentState.GrassPositions.Add(grassPosition)), CurrentState);
   }

   public GameLogic SpawnSheep(Point point)
   {
      return new GameLogic(worldWidth, worldHeight, new EntityManager(CurrentState.SheepPositions.Add(point), CurrentState.GrassPositions), CurrentState);
   }
}