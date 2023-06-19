using System.ComponentModel;

namespace Snek.Core;

public enum WallCollisionBehavior
{
    /// <summary>
    /// The game is over.
    /// </summary>
    [Description("The game is over")]
    GameOver,
    /// <summary>
    /// The players snake is reversed and continues traveling in the opposite direction.
    /// </summary>
    [Description("The players snake is reversed and continues traveling in the opposite direction")]
    Rebound,
    /// <summary>
    /// The player travels through walls. They will continue moving in the same direction
    /// but will emerge from the wall opposite the one they collided with.
    /// </summary>
    [Description("The player travels through walls. They will continue moving in the same direction but will emerge from the wall opposite the one they collided with")]
    Portal
}