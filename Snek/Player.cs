using Snek.Abstract;
using Snek.Extensions;
using Snek.Interfaces;

namespace Snek;

public class Player : StyledObject
{
    public override ConsoleColor BackgroundColor => ConsoleColor.White;
    public override ConsoleColor SpriteColor => ConsoleColor.Red;
    public override char Sprite => Facing.GetSprite();
    public Direction Facing { get; private set; }
    public List<Cell> Cells { get; }
    public Cell Head => Cells.First();
    public Cell Tail => Cells.Last();

    public Player(Position position)
    {
        Cells = new() { new Cell(position.X, position.Y, BackgroundColor, SpriteColor, Sprite) };
        Facing = Direction.North;
    }

    public bool CanFace(Direction direction)
        => !direction.IsOpposite(Facing);

    public void Face(Direction direction)
        => Facing = direction;

    /// <summary>
    /// The next position that the players head will occupy if they move one cell in the current direction.
    /// </summary>
    public Position NextHeadPosition() => Facing switch
    {
        Direction.North => new Position(Head.Position.X, Head.Position.Y - 1),
        Direction.East => new Position(Head.Position.X + 1, Head.Position.Y),
        Direction.South => new Position(Head.Position.X, Head.Position.Y + 1),
        Direction.West => new Position(Head.Position.X - 1, Head.Position.Y),
        _ => throw new Exception(),
    };

    public bool IsOccupyingPosition(Position position)
        => Cells.Any(c => c.Position == position);
}