using Snek.Abstract;

namespace Snek;

/// <summary>
/// The grid contains all game elements and triggers information to be drawn to the display.
/// </summary>
public class Grid : StyledObject
{
    /// <summary>
    /// Object that exposes the relevant information that's published when a <see cref="Snek.Cell"/> is updated.
    /// </summary>
    /// <param name="Cell">The cell that's been updated</param>
    public record CellUpdatedEventArgs(Cell Cell);

    /// <summary>
    /// The signature of the method that consumes Cell update events.
    /// </summary>
    /// <param name="sender">The thing that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void CellUpdatedEventHandler(object sender, CellUpdatedEventArgs e);

    public int Width { get; }
    public int Height { get; }

    public override ConsoleColor BackgroundColor => ConsoleColor.Black;
    public override ConsoleColor SpriteColor => ConsoleColor.Black;
    public override char Sprite => ' ';

    public List<Cell> Cells { get; } = new();
    private Player? _player;
    private Enemy? _enemy;
    private readonly Random _random = new();


    /// <summary>
    /// The event that is fired whenever a <see cref="Cell"/> has been updated.
    /// </summary>
    public event CellUpdatedEventHandler? CellUpdated;

    private IEnumerable<Position> AvailablePositions => Cells
        .Where(cell => (_enemy == null || cell.Position != _enemy.Cell.Position)
            && (!_player?.Cells?.Any(playerCell => playerCell.Position == cell.Position) ?? true))
        .Select(cell => cell.Position);

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < width; y++)
            {
                var cell = CreateCell(new Position(x, y));
                Cells.Add(cell);
                OnCellUpdated(cell);
            }
    }

    public Position GetRandomAvailablePosition()
        => AvailablePositions.ElementAt(_random.Next(0, AvailablePositions.Count() - 1));

    public void Add(Player player)
    {
        _player = player;
        _player.Cells.ForEach(OnCellUpdated);
    }

    public void Add(Enemy enemy)
    {
        _enemy = enemy;
        OnCellUpdated(enemy.Cell);
    }

    public bool IsInBounds(Position position)
        => position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;

    public void MovePlayer(Position nextHeadPosition)
    {
        ArgumentNullException.ThrowIfNull(_player);
        var newHeadCell = _player.CreateCell(nextHeadPosition);

        // In order to tell the display that the players tail is no longer on the current tail position
        // (i.e. the players snake has moved by 1 position), we need to create a cell using the grid style 
        // at the current position snakes tail.
        var oldTailCell = CreateCell(_player.Tail.Position);

        _player.Cells.Insert(0, newHeadCell);
        _player.Cells.Remove(_player.Tail);

        OnCellUpdated(newHeadCell);
        OnCellUpdated(oldTailCell);
    }

    /// <summary>
    /// Adds an extra cell to the end of the player.
    /// </summary>
    public void ExtendPlayerTail()
    {
        ArgumentNullException.ThrowIfNull(_player);
        var cell = _player.CreateCell(_player.Tail.Position);
        _player.Cells.Add(cell);
        OnCellUpdated(cell);
    }

    /// <summary>
    /// Invokes the event that's fired when a cell has been updated.
    /// </summary>
    /// <param name="cell">The cell that was updated</param>
    protected virtual void OnCellUpdated(Cell cell)
    {
        CellUpdated?.Invoke(this, new CellUpdatedEventArgs(cell));
    }
}