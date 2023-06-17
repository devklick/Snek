using Snek.Events;
using Snek.Interfaces;

namespace Snek;

/// <summary>
/// The <see cref="GameGrid"/> contains all game elements and triggers information to be drawn to the display.
/// </summary>
/// <remarks>
/// Note that this does not contain any information that is part of the <see cref="Hud"/>. 
/// </remarks>
public class GameGrid : IStyled<Cell>, IGrid
{
    public int Width { get; }
    public int Height { get; }
    public Position Offset => Position.Default;

    public ConsoleColor BackgroundColor => ConsoleColor.DarkGray;
    public ConsoleColor SpriteColor => ConsoleColor.DarkGray;
    public char Sprite => ' ';

    public List<Cell> Cells { get; } = new();
    private Player? _player;
    private Enemy? _enemy;
    private readonly Random _random = new();


    /// <summary>
    /// The event that is fired whenever a <see cref="Cell"/> has been updated.
    /// </summary>
    public event CellUpdatedEventHandler? CellUpdated;

    public IEnumerable<Position> AvailablePositions => Cells
        .Where(cell => (_enemy == null || cell.Position != _enemy.Cell.Position)
            && (_player == null || !_player.Cells.Any(playerCell => playerCell.Position == cell.Position)))
        .Select(cell => cell.Position);

    public GameGrid(int width, int height)
    {
        Width = width;
        Height = height;

        BuildGridCells();
    }

    public Position GetRandomAvailablePosition()
        => AvailablePositions.ElementAt(_random.Next(0, AvailablePositions.Count() - 1));

    public void Add(Player player)
    {
        _player = player;
        _player.Cells.ForEach(OnCellUpdated);
    }

    public void Add(Enemy? enemy)
    {
        _enemy = enemy;

        if (_enemy != null) OnCellUpdated(_enemy.Cell);
    }

    public bool IsInBounds(Position position)
        => position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;

    public Position MovePlayer(Position nextHeadPosition)
    {
        ArgumentNullException.ThrowIfNull(_player);

        // The new head position should have the colors flipped
        var newHeadCell = _player.CreateCell(nextHeadPosition, true, _player.Facing);

        // We need to update the cell that is currently the players head, 
        // Since it's no longer the head it no longer uses the head style
        var oldHeadCell = _player.CreateCell(_player.Head.Position, false, _player.Facing);

        // In order to tell the display that the players tail is no longer on the current tail position
        // (i.e. the players snake has moved by 1 position), we need to create a cell using the grid style 
        // at the current position snakes tail.
        var oldTailCell = CreateCell(_player.Tail.Position);

        // Add the new head cell, update the previous head cell, and remove the tail cell
        _player.Cells.Insert(0, newHeadCell);
        _player.Cells[1] = oldHeadCell;
        _player.Cells.Remove(_player.Tail);

        OnCellUpdated(oldTailCell);
        OnCellUpdated(oldHeadCell);
        OnCellUpdated(newHeadCell);

        return oldTailCell.Position;
    }

    /// <summary>
    /// Adds an extra cell to the end of the player.
    /// </summary>
    public void ExtendPlayerTail(Position position)
    {
        ArgumentNullException.ThrowIfNull(_player);
        var cell = _player.CreateCell(position);
        _player.Cells.Add(cell);
        OnCellUpdated(cell);
    }

    public void ReversePlayer()
    {
        ArgumentNullException.ThrowIfNull(_player);
        _player.Reverse();
        _player.Cells.ForEach(OnCellUpdated);
    }

    public void PortalPlayer(Position oldHeadPosition)
    {
        int x = oldHeadPosition.X, y = oldHeadPosition.Y;

        if (oldHeadPosition.X < 0) x = Width - 1;
        else if (oldHeadPosition.X > Width - 1) x = 0;
        else if (oldHeadPosition.Y < 0) y = Height - 1;
        else if (oldHeadPosition.Y > Height - 1) y = 0;

        MovePlayer(new Position(x, y));
    }

    public void Reset() => BuildGridCells();

    /// <summary>
    /// Invokes the event that's fired when a cell has been updated.
    /// </summary>
    /// <param name="cell">The cell that was updated</param>
    protected virtual void OnCellUpdated(Cell cell)
    {
        CellUpdated?.Invoke(this, new CellUpdatedEventArgs(cell));
    }

    private void BuildGridCells()
    {
        Cells.Clear();
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var cell = CreateCell(new Position(x, y));
                Cells.Add(cell);
                OnCellUpdated(cell);
            }
    }

    public void SetPlayerFacing(Direction direction)
    {
        ArgumentNullException.ThrowIfNull(_player);
        _player.Face(direction);
        OnCellUpdated(_player.CreateCell(_player.Head.Position, true, direction));

    }

    public Cell CreateCell(Position position)
        => new(position, BackgroundColor, SpriteColor, Sprite);
}