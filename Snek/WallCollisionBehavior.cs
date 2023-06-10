namespace Snek;

public enum WallCollisionBehavior
{
    /// <summary>
    /// The game is over.
    /// </summary>
    GameOver,
    /// <summary>
    /// The players snake is reversed, so there head is now where their tail was 
    /// and vis versa, and they're now facing the opposite direction.
    /// </summary>
    Rebound,
    /// <summary>
    /// The player travels through walls. They will continue moving in the same direction
    /// but will emerge from the opposite wall they collided with.
    /// </summary>
    Portal
}