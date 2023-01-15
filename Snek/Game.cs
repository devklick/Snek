using Snek.Extensions;

namespace Snek;

/// <summary>
/// The service responsible for initializing orchestrating the game.
/// </summary>
public class Game
{
    private readonly Grid _grid;
    private readonly Player _player;
    private Enemy _enemy;
    private readonly Display _display;
    private readonly InputManager _input;
    private GameState _state;
    private int _ticksPerSecond = 3;
    private int _delay => (int)((float)1 / _ticksPerSecond * 1000);
    private int _score = 0;

    public Game(int width, int height)
    {
        // The order of operations is important here.
        // Grid has to be created first, obviously
        _grid = new(width, height);

        // Next, the display needs to be created, so it knows about and draws the grid
        _display = new(_grid, 2, 1);

        // Next, the player needs to be created an added to the grid.
        // Doing so will update the display with the player cells 
        _player = new(new Position(width / 2, height / 2));
        _grid.Add(_player);

        // Now we can check all available positions, determine where the enemy should be positioned,
        // And add the enemy to the grid. Doing so will update the display with the enemy cell.
        _enemy = new(_grid.GetRandomAvailablePosition());
        _grid.Add(_enemy);

        // The order of the input manager isnt really important.
        _input = new();
    }

    /// <summary>
    /// Starts game play and carries out the game logic. 
    /// This method will not return until the gameplay is complete.
    /// </summary>
    public void Play()
    {
        _state = GameState.Playing;
        while (_state != GameState.Ended)
        {
            Thread.Sleep(_delay);

            var input = _input.GetInput();

            if (input == PlayerInput.TogglePause)
            {
                HandleTogglePause();
            }

            if (_state == GameState.Paused) continue;

            if (input.HasValue && input.Value.IsDirection(out var direction))
            {
                HandleChangeDirection(direction!.Value);
            }

            Tick();
        }
    }

    /// <summary>
    /// Toggles play/pause based on the current game state.
    /// </summary>
    private void HandleTogglePause()
    {
        if (_state == GameState.Playing) _state = GameState.Paused;
        else if (_state == GameState.Paused) _state = GameState.Playing;
    }

    /// <summary>
    /// Handles the players request to turn and face the specified <paramref name="direction"/>.
    /// </summary>
    /// <remarks>
    /// Note that the player can not turn to face *any* direction. 
    /// For example, the cannot face the direction opposite the direction they are currently facing.
    /// </remarks>
    /// <param name="direction">The direction the player wants to face</param>
    private void HandleChangeDirection(Direction direction)
    {
        if (!_player.CanFace(direction)) return;

        _player.Face(direction);
    }

    /// <summary>
    /// Performs a single cycle of the game logic, checking:
    /// <list type="bullet">
    ///     <item>Player out of bounds</item>
    ///     <item>Player collided with self</item>
    ///     <item>Player destroyed enemy</item>
    /// </list>
    /// </summary>
    private void Tick()
    {
        var nextHeadPosition = _player.NextHeadPosition();

        if (!_grid.IsInBounds(nextHeadPosition) || _player.IsOccupyingPosition(nextHeadPosition))
        {
            _state = GameState.Ended;
            return;
        }

        _grid.MovePlayer(nextHeadPosition);

        if (EnemyDestroyed())
        {
            HandleEnemyDestroyed();
        }
    }

    /// <summary>
    /// Determines whether or not the player has destroyed the current enemy.
    /// </summary>
    private bool EnemyDestroyed()
        => _player.Head.Position == _enemy.Cell.Position;

    /// <summary>
    /// Handles the functionality that needs to be executed when the player destroys an enemy.
    /// </summary>
    private void HandleEnemyDestroyed()
    {
        _grid.ExtendPlayerTail();
        _enemy = new(_grid.GetRandomAvailablePosition());
        _grid.Add(_enemy);
        _score++;
        _ticksPerSecond++;
    }
}