using System.Drawing;

namespace Moutons;

public class GameLogic
{
    private readonly int worldWidth;
    private readonly int worldHeight;

    private EntityManager CurrentState { get; }
    private EntityManager PreviousState { get; }

    public Point SheepPosition => CurrentState.SheepPosition;
    public Point PreviousSheepPosition => PreviousState.SheepPosition;
    public bool SheepPositionChanged => SheepPosition != PreviousSheepPosition;

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

    public GameLogic MoveSheepLeft()
    {
        int newX = Math.Max(1, CurrentState.SheepPosition.X - 1);
        Point newPosition = new Point(newX, CurrentState.SheepPosition.Y);
        return new GameLogic(worldWidth, worldHeight, new EntityManager(newPosition), CurrentState);
    }

    public GameLogic MoveSheepRight()
    {
        int newX = Math.Min(worldWidth - 2, CurrentState.SheepPosition.X + 1);
        Point newPosition = new Point(newX, CurrentState.SheepPosition.Y);
        return new GameLogic(worldWidth, worldHeight, new EntityManager(newPosition), CurrentState);
    }

    public GameLogic MoveSheepUp()
    {
        int newY = Math.Max(1, CurrentState.SheepPosition.Y - 1);
        Point newPosition = new Point(CurrentState.SheepPosition.X, newY);
        return new GameLogic(worldWidth, worldHeight, new EntityManager(newPosition), CurrentState);
    }

    public GameLogic MoveSheepDown()
    {
        int newY = Math.Min(worldHeight - 2, CurrentState.SheepPosition.Y + 1);
        Point newPosition = new Point(CurrentState.SheepPosition.X, newY);
        return new GameLogic(worldWidth, worldHeight, new EntityManager(newPosition), CurrentState);
    }
}
