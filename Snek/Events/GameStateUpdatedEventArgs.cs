namespace Snek.Events;

public class GameStateUpdatedEventArgs
{
    public GameState State { get; }
    public GameStateUpdatedEventArgs(GameState state)
    {
        State = state;
    }
}