using Snek.Attributes;

namespace Snek.Input;

/// <summary>
/// The input request that the player has made.
/// </summary>
public enum PlayerInput
{
    [MapToDirection(Direction.North)]
    FaceNorth,

    [MapToDirection(Direction.East)]
    FaceEast,

    [MapToDirection(Direction.South)]
    FaceSouth,

    [MapToDirection(Direction.West)]
    FaceWest,

    TogglePause,

    Replay,

    Quit
}