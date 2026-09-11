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

    public Point SheepPosition => CurrentState.SheepPosition;
    public Point PreviousSheepPosition => PreviousState.SheepPosition;
    public bool SheepPositionChanged => SheepPosition != PreviousSheepPosition;
    public bool StateChanged => SheepPositionChanged;

    public GameLogic(int worldWidth, int worldHeight, Point initialPosition)
    {
        this.worldWidth = worldWidth;
        this.worldHeight = worldHeight;
        CurrentState = new EntityManager(initialPosition);
        PreviousState = new EntityManager(initialPosition);
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

        int newX = Math.Clamp(CurrentState.SheepPosition.X + delta.X, 1, worldWidth - 2);
        int newY = Math.Clamp(CurrentState.SheepPosition.Y + delta.Y, 1, worldHeight - 2);
        Point newPosition = new Point(newX, newY);
        return new GameLogic(worldWidth, worldHeight, new EntityManager(newPosition), CurrentState);
    }
}
