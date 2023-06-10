using Snek.Events;
using Snek.Extensions;

namespace Snek;

/// <summary>
/// The service responsible for initializing orchestrating the game.
/// </summary>
public class Game
{
    private readonly GameGrid _grid;
    private Player _player;
    private readonly Display _display;
    private readonly Hud _hud;
    private readonly InputManager _input;
    private readonly GamePlayTimer _timer;
    private Enemy? _enemy;
    private GameState _state;

    private int _ticksPerSecond;
    private int _score = 0;
    private event ScoreUpdatedEventHandler? ScoreUpdated;
    private event GameStateUpdatedEventHandler? GameStateUpdated;
    private int Delay => (int)((float)1 / _ticksPerSecond * 1000);
    private readonly GameSettings _settings;

    public Game(GameSettings settings)
    {
        _settings = settings;
        _ticksPerSecond = _settings.InitialTicksPerSecond;

        // Set up the game timer to tick every 1 second
        _timer = new GamePlayTimer(1000);

        _input = new();

        // The order of operations is important here.
        // Grid has to be created first, obviously
        _grid = new(_settings.Width, _settings.Height);

        // The HUD is drawn below the game grid, and it's cells do not get multiplied. 
        // However we want it's width to the the same as the multiplied grid dimensions.
        // The idea of passing in event's as references isnt great, but it gets the job done.
        _hud = new Hud(settings.HudWidth, settings.HudHeight, new Position(0, _grid.Width), _input, ref ScoreUpdated, ref _timer, ref GameStateUpdated);

        // Next, the display needs to be created, so it knows about and draws the grid
        _display = new(settings.DisplayWidth, settings.DisplayHeight, settings.DisplayWidthMultiplier, settings.DisplayHeightMultiplier, _grid, _hud);

        // Since we've got the display configured and rendered, we can trigger the update of the various info that gets drawn to the HUD.
        SetGameState(GameState.Initializing);
        SetScore(0);
        _timer.Reset();

        // Next, the player needs to be created an added to the grid.
        // Doing so will update the display with the player cells.
        _player = new(new Position(_settings.Width / 2, _settings.Height / 2));
        _grid.Add(_player);

        // Now we can check all available positions, determine where the enemy should be positioned,
        // And add the enemy to the grid. Doing so will update the display with the enemy cell.
        _enemy = new(_grid.GetRandomAvailablePosition());
        _grid.Add(_enemy);
    }

    private void Reset()
    {
        _timer.Reset();
        _grid.Reset();
        _hud.Reset();
        SetGameState(GameState.Initializing);
        SetScore(0);
        _timer.Reset();
        _player = new(new Position(_settings.Width / 2, _settings.Height / 2));
        _grid.Add(_player);
        _enemy = new(_grid.GetRandomAvailablePosition());
        _grid.Add(_enemy);
        _ticksPerSecond = _settings.InitialTicksPerSecond;
    }

    /// <summary>
    /// Starts game play and carries out the game logic. 
    /// This method will not return until the gameplay is complete.
    /// </summary>
    public void Play()
    {
        GameLoop();
        ReplayLoop();
    }

    private void GameLoop()
    {
        SetGameState(GameState.Playing);
        while (!_state.IsGameplayOver())
        {
            Thread.Sleep(Delay);

            var input = _input.GetInput(_state);

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

    private void ReplayLoop()
    {
        while (_state.IsGameplayOver())
        {
            var input = _input.GetInput(_state);

            if (input == PlayerInput.Replay)
            {
                SetGameState(GameState.Initializing);
                Reset();
                Play();
            }
            else if (input == PlayerInput.Quit)
            {
                _state = GameState.Exiting;
            }
        }
    }

    /// <summary>
    /// Toggles play/pause based on the current game state.
    /// </summary>
    private void HandleTogglePause()
    {
        if (_state == GameState.Playing)
        {
            SetGameState(GameState.Paused);
        }
        else if (_state == GameState.Paused)
        {
            SetGameState(GameState.Playing);
        }
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

        _grid.SetPlayerFacing(direction);
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

        if (!_grid.IsInBounds(nextHeadPosition))
        {
            HandleWallCollision(nextHeadPosition);
            return;
        }

        if (_player.IsOccupyingPosition(nextHeadPosition, true))
        {
            SetGameState(GameState.GameOver);
            return;
        }

        var oldTailPosition = _grid.MovePlayer(nextHeadPosition);

        if (EnemyDestroyed())
        {
            HandleEnemyDestroyed(oldTailPosition);
        }
    }

    private void HandleWallCollision(Position nextHeadPosition)
    {
        switch (_settings.WallCollisionBehavior)
        {
            case WallCollisionBehavior.Rebound:
                _grid.ReversePlayer();
                break;
            case WallCollisionBehavior.Portal:
                _grid.PortalPlayer(nextHeadPosition);
                break;

            case WallCollisionBehavior.GameOver:
            default:
                SetGameState(GameState.GameOver);
                break;
        }
    }

    /// <summary>
    /// Determines whether or not the player has destroyed the current enemy.
    /// </summary>
    private bool EnemyDestroyed()
        => _enemy != null && _player.Head.Position == _enemy.Cell.Position;

    /// <summary>
    /// Handles the functionality that needs to be executed when the player destroys an enemy.
    /// </summary>
    private void HandleEnemyDestroyed(Position oldTailPosition)
    {
        _enemy = null;
        _grid.Add(_enemy);
        _grid.ExtendPlayerTail(oldTailPosition);

        if (_grid.AvailablePositions.Any())
        {
            _enemy = new(_grid.GetRandomAvailablePosition());
            _grid.Add(_enemy);

            SetScore(_score + 1);

            if (_settings.IncreaseSpeedOnEnemyDestroyed)
            {
                _ticksPerSecond++;
            }
        }
        else
        {
            SetGameState(GameState.Won);
        }
    }

    /// <summary>
    /// Sets the current score, firing an update event.
    /// </summary>
    /// <param name="score">The new score value</param>
    private void SetScore(int score)
    {
        _score = score;
        ScoreUpdated?.Invoke(this, new ScoreUpdatedEventArgs(score));
    }

    /// <summary>
    /// Sets the game state, taking care of starting/stopping the timer and firing an update event.
    /// </summary>
    /// <param name="state">The new game state</param>
    private void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.GameOver:
            case GameState.Paused:
            case GameState.Won:
                _timer.Stop();
                break;
            case GameState.Playing:
                _timer.Start();
                break;
        }
        _state = state;
        GameStateUpdated?.Invoke(this, new(_state));
    }
}