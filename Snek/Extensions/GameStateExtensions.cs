namespace Snek;

public static class GameStateExtensions
{
    public static bool IsGameplayOver(this GameState state)
        => state == GameState.Won || state == GameState.GameOver;
}