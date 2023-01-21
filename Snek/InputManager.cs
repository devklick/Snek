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

    TogglePause,

    Replay,

    Quit
}

public class InputManager
{
    // TODO: Would be nice to have this configurable rather than hardcoded.
    /// <summary>
    /// A map between input keys an input actions.
    /// </summary>
    private readonly Dictionary<(GameState GameState, ConsoleKey Key), PlayerInput> _KapToActionMap;
    private readonly Dictionary<PlayerInput, IEnumerable<(GameState GameState, ConsoleKey Key)>> _actionToKeyMap;

    public InputManager()
    {
        _KapToActionMap = new()
        {
            { (GameState.Playing, ConsoleKey.UpArrow), PlayerInput.FaceNorth },
            { (GameState.Playing, ConsoleKey.W), PlayerInput.FaceNorth },

            { (GameState.Playing, ConsoleKey.LeftArrow), PlayerInput.FaceWest },
            { (GameState.Playing, ConsoleKey.A), PlayerInput.FaceWest },

            { (GameState.Playing, ConsoleKey.DownArrow), PlayerInput.FaceSouth },
            { (GameState.Playing, ConsoleKey.S), PlayerInput.FaceSouth},

            { (GameState.Playing, ConsoleKey.RightArrow), PlayerInput.FaceEast },
            { (GameState.Playing, ConsoleKey.D), PlayerInput.FaceEast},

            { (GameState.Playing, ConsoleKey.Escape), PlayerInput.TogglePause },
            { (GameState.Paused, ConsoleKey.Escape), PlayerInput.TogglePause },

            { (GameState.GameOver, ConsoleKey.R), PlayerInput.Replay },
            { (GameState.GameOver, ConsoleKey.Escape), PlayerInput.Quit },
            { (GameState.Won, ConsoleKey.R), PlayerInput.Replay },
            { (GameState.Won, ConsoleKey.Escape), PlayerInput.Quit },
        };

        _actionToKeyMap = new();

        foreach (var entry in _KapToActionMap)
        {
            var action = entry.Value;
            var mapping = entry.Key;

            if (!_actionToKeyMap.TryAdd(action, new[] { mapping }))
            {
                var mappings = _actionToKeyMap[action].Append(mapping);
                _actionToKeyMap[entry.Value] = mappings;
            }
        }
    }

    /// <summary>
    /// Checks if the player has pressed any known key and returns the <see cref="PlayerInput"/> associated with the known key.
    /// If no key was pressed, or an unknown key was pressed, `null` is returned.
    /// </summary>
    public PlayerInput? GetInput(GameState currentGameState)
    {
        if (Console.KeyAvailable)
        {
            var keyInfo = Console.ReadKey(true);

            if (_KapToActionMap.TryGetValue((currentGameState, keyInfo.Key), out var input))
            {
                return input;
            }
        }

        return null;
    }

    public IEnumerable<(GameState GameState, ConsoleKey Key)> GetMappingForInput(PlayerInput input)
    {
        _actionToKeyMap.TryGetValue(input, out var mapping);
        return mapping ?? Array.Empty<(GameState GameState, ConsoleKey Key)>();
    }
}