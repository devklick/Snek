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
        _gamePlayTimerTextBox = new TextBox(new Position(1, 1), Alignment.Right, BackgroundColor, SpriteColor, "Time");
    }

    private void GamePlayTimerUpdated(object? sender, GamePlayTimerUpdatedEventArgs e)
        => OnTextBoxUpdated(_gamePlayTimerTextBox, Math.Round(e.Elapsed.TotalSeconds, 0).ToString() + "s");

    public void OnGameScoreUpdated(object? sender, ScoreUpdatedEventArgs e)
        => OnTextBoxUpdated(_scoreTextBox, e.Score.ToString());

    private void OnTextBoxUpdated(TextBox textBox, string value)
    {
        textBox.SetValue(value);

        foreach (var cell in GetTextBoxContentCells(textBox))
        {
            _cells[(cell.Position.X, cell.Position.Y)] = cell;
            CellUpdated?.Invoke(this, new CellUpdatedEventArgs(cell, true));
        }
    }

    private IEnumerable<Cell> GetTextBoxContentCells(TextBox textBox)
    {
        var y = textBox.Anchor.Y;
        for (int i = 0; i < textBox.Content.Length; i++)
        {
            var sprite = textBox.Content.ElementAt(i);
            var x = GetXPositionForValueAtIndex(textBox, i);
            yield return new Cell(x, y, textBox.BackgroundColor, textBox.ForegroundColor, sprite);
        }
    }

    private int GetXPositionForValueAtIndex(TextBox textBox, int index)
        => textBox.Align switch
        {
            Alignment.Left => textBox.Anchor.X + index,
            Alignment.Right => Width - textBox.Anchor.X - textBox.Content.Length + index,
            _ => (Width / 2) - (textBox.Content.Length / 2) + index
        };
}