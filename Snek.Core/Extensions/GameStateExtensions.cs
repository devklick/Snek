namespace Snek.Core;

public static class GameStateExtensions
{
    extension(GameState state)
    {
        public bool IsOver => state == GameState.Won || state == GameState.GameOver;
    }
}