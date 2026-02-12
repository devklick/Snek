namespace Snek.Core.Events;

public class GameStateUpdatedEventArgs
{
    public GameState GameState { get; }
    public GameStateUpdatedEventArgs(GameState state)
    {
        GameState = state;
    }
}