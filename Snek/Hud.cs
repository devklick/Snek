using Snek.Extensions;
using Snek.Events;
using Snek.Interfaces;

namespace Snek;

/// <summary>
/// A panel that exists as part of a <see cref="Display"/> where information can be presented.
/// </summary>
public class Hud : IStyled<Cell>, IGrid
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
    public ConsoleColor BackgroundColor { get; }
    public ConsoleColor SpriteColor { get; }
    public char Sprite => ' ';

    private readonly Dictionary<(int x, int y), Cell> _cells = new();
    private readonly InputManager _input;
    private readonly TextBox _scoreTextBox;
    private readonly TextBox _gamePlayTimerTextBox;
    private readonly TextBox _gameStateTextBox;

    public event CellUpdatedEventHandler? CellUpdated;

    public Hud(int width, int height,
        Position anchor,
        InputManager input,
        ref ScoreUpdatedEventHandler? scoreUpdatedEventHandler,
        ref GamePlayTimer gamePlayTimer,
        ref GameStateUpdatedEventHandler? gameStateUpdatedEventHandler)
    {
        _input = input;
        Width = width;
        Height = height;
        Anchor = anchor;
        BackgroundColor = ConsoleColor.DarkGray;
        SpriteColor = ConsoleColor.White;
        scoreUpdatedEventHandler += OnGameScoreUpdated;
        gamePlayTimer.Updated += GamePlayTimerUpdated;
        gameStateUpdatedEventHandler += OnGameStateUpdated;

        BuildHudCells();

        _scoreTextBox = new TextBox(new Position(2, 3), Alignment.Left, BackgroundColor, SpriteColor, "Score");
        _gamePlayTimerTextBox = new TextBox(new Position(2, 3), Alignment.Right, BackgroundColor, SpriteColor, "Time");
        _gameStateTextBox = new TextBox(new Position(0, 1), Alignment.Centre, BackgroundColor, SpriteColor);
    }

    public void Reset() => BuildHudCells();

    private void GamePlayTimerUpdated(object? sender, GamePlayTimerUpdatedEventArgs e)
        => UpdateTextBox(_gamePlayTimerTextBox, Math.Round(e.Elapsed.TotalSeconds, 0).ToString() + "s");

    private void OnGameScoreUpdated(object? sender, ScoreUpdatedEventArgs e)
        => UpdateTextBox(_scoreTextBox, e.Score.ToString());

    private void OnGameStateUpdated(object? sender, GameStateUpdatedEventArgs e)
    {
        var message = e.State.ToString().ToSentenceCase();
        if (e.State.IsGameplayOver())
        {
            var mapping = _input.GetMappingForInput(PlayerInput.Replay);
            if (mapping.Any())
            {
                message += $" - Replay? ({mapping.First().Key.ToString().ToLower()})";
            }
        }
        UpdateTextBox(_gameStateTextBox, message);
    }

    private void UpdateTextBox(TextBox textBox, string value)
    {
        // if the current value is longer than the new value, we need to "reset" the cells that the new value will not overwrite.
        if (textBox.Value != null && textBox.Value.Length > value.Length)
        {
            foreach (var cell in GetTextBoxContentAsCells(textBox.Anchor, textBox.Align, textBox.Content, textBox.BackgroundColor, textBox.ForegroundColor, Sprite))
            {
                UpdateCell(cell);
            }
        }

        textBox.SetValue(value);

        foreach (var cell in GetTextBoxContentAsCells(textBox.Anchor, textBox.Align, textBox.Content, textBox.BackgroundColor, textBox.ForegroundColor))
        {
            UpdateCell(cell);
        }
    }

    private void UpdateCell(Cell cell)
    {
        _cells[(cell.Position.X, cell.Position.Y)] = cell;
        CellUpdated?.Invoke(this, new CellUpdatedEventArgs(cell, true));
    }

    private IEnumerable<Cell> GetTextBoxContentAsCells(Position anchor, Alignment align, string? content, ConsoleColor backgroundColor, ConsoleColor foregroundColor, char? sprite = null)
    {
        if (content == null) yield break;
        var y = anchor.Y;
        for (int i = 0; i < content.Length; i++)
        {
            var spriteToUse = sprite ?? content.ElementAt(i);
            var x = GetXPositionForValueAtIndex(anchor, align, content, i);
            yield return new Cell(x, y, backgroundColor, foregroundColor, spriteToUse);
        }
    }

    private int GetXPositionForValueAtIndex(Position anchor, Alignment align, string content, int index)
        => align switch
        {
            Alignment.Left => anchor.X + index,
            Alignment.Right => Width - anchor.X - content.Length + index,
            _ => (Width / 2) - (content.Length / 2) + index
        };

    private void BuildHudCells()
    {
        _cells.Clear();
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                _cells.Add((x, y), new(x, y, BackgroundColor, SpriteColor, Sprite));
    }

    public Cell CreateCell(Position position)
        => new(position, BackgroundColor, SpriteColor, Sprite);
}