using Snek.Attributes;

namespace Snek;


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

    TogglePause
}

public class InputManager
{
    // TODO: Would be nice to have this configurable rather than hardcoded.
    /// <summary>
    /// A map between input keys an input actions.
    /// </summary>
    private readonly Dictionary<ConsoleKey, PlayerInput> _keyMap = new()
    {
        { ConsoleKey.UpArrow, PlayerInput.FaceNorth },
        { ConsoleKey.W, PlayerInput.FaceNorth },

        { ConsoleKey.LeftArrow, PlayerInput.FaceWest },
        { ConsoleKey.A, PlayerInput.FaceWest },

        { ConsoleKey.DownArrow, PlayerInput.FaceSouth },
        { ConsoleKey.S, PlayerInput.FaceSouth},

        { ConsoleKey.RightArrow, PlayerInput.FaceEast },
        { ConsoleKey.D, PlayerInput.FaceEast},

        { ConsoleKey.Escape, PlayerInput.TogglePause },
    };


    /// <summary>
    /// Checks if the player has pressed any known key and returns the <see cref="PlayerInput"/> associated with the known key.
    /// If no key was pressed, or an unknown key was pressed, `null` is returned.
    /// </summary>
    public PlayerInput? GetInput()
    {
        if (Console.KeyAvailable)
        {
            var keyInfo = Console.ReadKey(true);

            if (_keyMap.TryGetValue(keyInfo.Key, out var input))
            {
                return input;
            }
        }

        return null;
    }
}