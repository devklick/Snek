using System.Timers;
using Snek.Abstract;
using Snek.Events;
using Snek.Interfaces;

namespace Snek;

/// <summary>
/// A panel that exists as part of a <see cref="Display"/> where information can be presented.
/// </summary>
public class Hud : StyledObject, IGrid
{
    /// <summary>
    /// The width of the HUD, which should match the width of the enclosing Display.
    /// Note that the HUD cells do not get multiplied on the Display, therefor if the GameGrid cells multiplied, 
    /// the HUD width should be the size of the multiplied GameGrid width.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The height of the HUD, which is determined by the amount of information that is intended to be presented in it.
    /// Note that this height is not multiplied when drawn to the Display.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The anchor is the point on the Display that the HUD is fixed to. 
    /// Any cells that exist as part of the HUD will be drawn to the Display relative to this anchor. 
    /// </summary>
    public Position Anchor { get; }

    public List<Cell> Cells => _cells.Values.ToList();
    public override ConsoleColor BackgroundColor { get; }
    public override ConsoleColor SpriteColor { get; }
    public override char Sprite => ' ';

    private readonly Dictionary<(int x, int y), Cell> _cells = new();
    private readonly TextBox _scoreTextBox;
    private readonly TextBox _gamePlayTimerTextBox;

    public event CellUpdatedEventHandler? CellUpdated;

    public Hud(int width, int height, Position anchor,
        ref ScoreUpdatedEventHandler? scoreUpdatedEventHandler,
        ref GamePlayTimer gamePlayTimer)
    {
        Width = width;
        Height = height;
        Anchor = anchor;
        BackgroundColor = ConsoleColor.DarkBlue;
        SpriteColor = ConsoleColor.Black;
        scoreUpdatedEventHandler += OnGameScoreUpdated;
        gamePlayTimer.Updated += GamePlayTimerUpdated;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _cells.Add((x, y), new(x, y, BackgroundColor, SpriteColor, Sprite));

        _scoreTextBox = new TextBox(new Position(1, 1), Alignment.Left, BackgroundColor, SpriteColor, "Score");
        _gamePlayTimerTextBox = new TextBox(new Position(1, 2), Alignment.Left, BackgroundColor, SpriteColor, "Time");
    }

    private void GamePlayTimerUpdated(object? sender, GamePlayTimerUpdatedEventArgs e)
    {
        _gamePlayTimerTextBox.SetValue(((int)e.Elapsed.TotalSeconds).ToString() + "s");

        var y = _gamePlayTimerTextBox.Anchor.Y;
        for (int i = 0; i < _gamePlayTimerTextBox.Content.Length; i++)
        {
            var sprite = _gamePlayTimerTextBox.Content.ElementAt(i);
            var x = _gamePlayTimerTextBox.Anchor.X + i;
            var cell = new Cell(x, y, _gamePlayTimerTextBox.BackgroundColor, _gamePlayTimerTextBox.ForegroundColor, sprite);
            _cells[(x, y)] = cell;
            CellUpdated?.Invoke(this, new CellUpdatedEventArgs(cell, true));
        }
    }

    public void OnGameScoreUpdated(object? sender, ScoreUpdatedEventArgs e)
    {
        _scoreTextBox.SetValue(e.Score.ToString());

        var y = _scoreTextBox.Anchor.Y;
        for (int i = 0; i < _scoreTextBox.Content.Length; i++)
        {
            var sprite = _scoreTextBox.Content.ElementAt(i);
            var x = _scoreTextBox.Anchor.X + i;
            var cell = new Cell(x, y, _scoreTextBox.BackgroundColor, _scoreTextBox.ForegroundColor, sprite);
            _cells[(x, y)] = cell;
            CellUpdated?.Invoke(this, new CellUpdatedEventArgs(cell, true));
        }
    }
}