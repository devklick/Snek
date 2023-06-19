using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Snek.Core.Audio;
using Snek.Core.Entities;
using Snek.Core.Events;
using Snek.Core.Extensions;
using Snek.Core.Infrastructure;
using Snek.Core.Input;
using Snek.Core.Settings;
using Snek.Core.UI;

namespace Snek.Core;

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
        _logger = new FileLogger(_settings.DebugLogging);
        _logger.LogInfo(nameof(Initialize), "Setting up game", _settings);

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
    private void Initialize(bool initLogs = false)
    {
        if (initLogs) _logger.Initialize();

        _grid.Reset();
        _hud.Reset();

        // Since we've got the display configured and rendered, we can trigger the update of the various info that gets drawn to the HUD.
        SetGameState(GameState.Initializing);
        SetScore(0);
        _timer.Reset();

        // Next, the player needs to be created an added to the grid.
        // Doing so will update the display with the player cells.
        InitializePlayer();

        // Now we can check all available positions, determine where the enemy should be positioned,
        // And add the enemy to the grid. Doing so will update the display with the enemy cell.
        InitializeEnemy();

        _ticksPerSecond = _settings.InitialTicksPerSecond;
    }

    [MemberNotNull(nameof(_player))]
    private void InitializePlayer()
    {
        _player = new(_logger, new Position(_settings.Width / 2, _settings.Height / 2));
        _logger.LogInfo(GetLogEventType(), "Adding player to grid", _player);
        _grid.Add(_player);
    }

    private void GameLoop()
    {
        SetGameState(GameState.Playing);

        while (!_state.IsGameplayOver())
        {
            _logger.LogInfo(GetLogEventType(), "Begin wait", Delay);
            Thread.Sleep(Delay);
            _logger.LogInfo(GetLogEventType(), "End wait");

            var input = _input.GetInput(_state);

            if (input != null)
            {
                _logger.LogInfo(GetLogEventType(), $"Player input captured", input);
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
                _logger.LogInfo(GetLogEventType(), "Player input captured", input);
            }

            if (input == PlayerInput.Replay)
            {
                _logger.LogInfo(GetLogEventType(), "Initializing replay");
                SetGameState(GameState.Initializing);
                Initialize(true);
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
        _logger.LogInfo(GetLogEventType(), "Checking if player can change direction", direction, _player);

        if (!_player.CanFace(direction))
        {
            _logger.LogInfo(GetLogEventType(), "Can not face direction", direction);
            return;
        }

        _grid.SetPlayerFacing(direction);

        _logger.LogInfo(GetLogEventType(), $"Player direction changed", direction);

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
        _logger.LogInfo(GetLogEventType(), "Next head position determined", nextHeadPosition);

        if (!_grid.IsInBounds(nextHeadPosition))
        {
            _logger.LogInfo(GetLogEventType(), "Next head position is out of bounds");
            HandleWallCollision(nextHeadPosition);
            return;
        }

        if (_player.IsOccupyingPosition(nextHeadPosition, true))
        {
            _logger.LogInfo(GetLogEventType(), "Player collided with self");
            _audio.PlayPlayerDestroyedSound();
            SetGameState(GameState.GameOver);
            return;
        }

        var oldTailPosition = _grid.MovePlayer(nextHeadPosition);

        if (EnemyDestroyed())
        {
            _logger.LogInfo(GetLogEventType(), "Enemy destroyed");
            HandleEnemyDestroyed(oldTailPosition);
        }
    }

    private void HandleWallCollision(Position nextHeadPosition)
    {
        switch (_settings.WallCollisionBehavior)
        {
            case WallCollisionBehavior.Rebound:
                _logger.LogInfo(GetLogEventType(), "Rebounding off wall");
                _grid.ReversePlayer();
                break;
            case WallCollisionBehavior.Portal:
                _grid.PortalPlayer(nextHeadPosition);
                _logger.LogInfo(GetLogEventType(), "Portalling through wall");
                break;

            case WallCollisionBehavior.GameOver:
            default:
                _logger.LogInfo(GetLogEventType(), "Crashed into wall");
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

        SetScore(_score + 1);

        if (_settings.IncreaseSpeedOnEnemyDestroyed)
        {
            _ticksPerSecond++;
            _logger.LogInfo(GetLogEventType(), $"Speed increased to {_ticksPerSecond} ticks per second");
        }

        InitializeEnemy();
    }

    [MemberNotNull(nameof(_enemy))]
    private void InitializeEnemy()
    {
        if (_grid.AvailablePositions.Any())
        {
            var newPosition = _grid.GetRandomAvailablePosition();
            _logger.LogInfo(GetLogEventType(), $"New enemy being placed at {newPosition}");
            _enemy = new(newPosition);
            _grid.Add(_enemy);
        }
        else
        {
            _logger.LogInfo(GetLogEventType(), "Snake takes up 100% of the grid");
            SetGameState(GameState.Won);
            _enemy = new(Position.Default);
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
        _logger.LogInfo(GetLogEventType(), $"Score updated", _score);
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
        _logger.LogInfo(GetLogEventType(), $"Game state updated", _state);
        GameStateUpdated?.Invoke(this, new(_state));
    }

    private string GetLogEventType([CallerMemberName] string memberName = "")
        => $"{nameof(Game)}.{memberName}";
}