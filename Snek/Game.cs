using System.Diagnostics.CodeAnalysis;
using Snek.Audio;
using Snek.Events;
using Snek.Extensions;
using Snek.Infrastructure;
using Snek.Settings;
using Snek.UI;

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
    private readonly AudioManager _audio;
    private readonly FileLogger _logger;

    public Game(GameSettings settings)
    {
        _settings = settings;
        _settings.ToString();

        _logger = new FileLogger();
        _logger.LogInfo("Setting up game", _settings);

        _audio = new AudioManager(settings.AudioEnabled);
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
        _hud = new Hud(settings.HudWidth, settings.HudHeight, new Position(0, _grid.Height), _input, ref ScoreUpdated, ref _timer, ref GameStateUpdated);

        // Next, the display needs to be created, so it knows about and draws the grid
        _display = new(settings.DisplayWidth, settings.DisplayHeight, settings.DisplayWidthMultiplier, settings.DisplayHeightMultiplier, _grid, _hud);

        // Finally, we initialize the various game components that get re-initialized on replay.
        Initialize();
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

    [MemberNotNull(nameof(_player)), MemberNotNull(nameof(_enemy))]
    private void Initialize()
    {
        _logger.Initialize();

        _grid.Reset();
        _hud.Reset();

        // Since we've got the display configured and rendered, we can trigger the update of the various info that gets drawn to the HUD.
        SetGameState(GameState.Initializing);
        SetScore(0);
        _timer.Reset();

        // Next, the player needs to be created an added to the grid.
        // Doing so will update the display with the player cells.
        _player = new(_logger, new Position(_settings.Width / 2, _settings.Height / 2));
        _logger.LogInfo(_player.ToString());
        _grid.Add(_player);

        // Now we can check all available positions, determine where the enemy should be positioned,
        // And add the enemy to the grid. Doing so will update the display with the enemy cell.
        _enemy = new(_grid.GetRandomAvailablePosition());
        _logger.LogInfo(_enemy.ToString());
        _grid.Add(_enemy);

        _ticksPerSecond = _settings.InitialTicksPerSecond;
    }

    private void GameLoop()
    {
        SetGameState(GameState.Playing);
        _logger.LogInfo("Gameplay started");

        while (!_state.IsGameplayOver())
        {
            _logger.LogInfo("Begin wait");
            Thread.Sleep(Delay);
            _logger.LogInfo("End wait");

            var input = _input.GetInput(_state);

            if (input != null)
            {
                _logger.LogInfo($"Player input {input}");
            }

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

            if (input != null)
            {
                _logger.LogInfo($"Player input {input}");
            }

            if (input == PlayerInput.Replay)
            {
                _logger.LogInfo($"Initializing replay");
                SetGameState(GameState.Initializing);
                Initialize();
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
        _logger.LogInfo(_player.ToString());
        _logger.LogInfo($"Player wants to face {direction}");

        if (!_player.CanFace(direction))
        {
            _logger.LogInfo($"Player unable to face {direction}");
            return;
        }

        _grid.SetPlayerFacing(direction);
        _logger.LogInfo($"Player now facing {direction}");

        _audio.PlayPlayerMovedSound();
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
        _logger.LogInfo($"Next head position is {nextHeadPosition}");

        if (!_grid.IsInBounds(nextHeadPosition))
        {
            _logger.LogInfo("Next head position is out of bounds");
            HandleWallCollision(nextHeadPosition);
            return;
        }

        if (_player.IsOccupyingPosition(nextHeadPosition, true))
        {
            _logger.LogInfo("Player collided with self");
            _audio.PlayPlayerDestroyedSound();
            SetGameState(GameState.GameOver);
            return;
        }

        var oldTailPosition = _grid.MovePlayer(nextHeadPosition);

        if (EnemyDestroyed())
        {
            _logger.LogInfo("Enemy destroyed");
            HandleEnemyDestroyed(oldTailPosition);
        }
    }

    private void HandleWallCollision(Position nextHeadPosition)
    {
        switch (_settings.WallCollisionBehavior)
        {
            case WallCollisionBehavior.Rebound:
                _logger.LogInfo("Rebounding off wall");
                _grid.ReversePlayer();
                break;
            case WallCollisionBehavior.Portal:
                _grid.PortalPlayer(nextHeadPosition);
                _logger.LogInfo("Portalling through wall");
                break;

            case WallCollisionBehavior.GameOver:
            default:
                _logger.LogInfo("Crashed into wall");
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
        _audio.PlayEnemyEatenSound();

        if (_grid.AvailablePositions.Any())
        {
            var newPosition = _grid.GetRandomAvailablePosition();
            _logger.LogInfo($"New enemy being placed at {newPosition}");
            _enemy = new(newPosition);
            _grid.Add(_enemy);

            SetScore(_score + 1);

            if (_settings.IncreaseSpeedOnEnemyDestroyed)
            {
                _ticksPerSecond++;
                _logger.LogInfo($"Speed increased to {_ticksPerSecond} ticks per second");
            }
        }
        else
        {
            _logger.LogInfo("Snake takes up 100% of the grid");
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
        _logger.LogInfo($"Score set to {_score}");
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
        _logger.LogInfo($"Game state updated to {_state}");
        GameStateUpdated?.Invoke(this, new(_state));
    }
}