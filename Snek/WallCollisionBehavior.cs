namespace Snek;

public enum WallCollisionBehavior
{
    GameOver,

    // TODO: Still a bit buggy where after rebounding, the head of the snake can attempt to move to it's neighboring cell position.
    Rebound
}