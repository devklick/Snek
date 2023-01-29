using Snek.Extensions;
using Snek.Interfaces;

namespace Snek;

public class Player : IStyled<PlayerCell>
{
    public ConsoleColor BackgroundColor => ConsoleColor.White;
    public ConsoleColor SpriteColor => ConsoleColor.Red;
    public char Sprite => Facing.GetSprite();
    public Direction Facing { get; private set; }
    public List<PlayerCell> Cells { get; }
    public PlayerCell Head => Cells.First();
    public PlayerCell Tail => Cells.Last();

    public Player(Position position, int initialLength = 5)
    {
        Cells = new();
        Facing = Direction.North;
        for (var i = 0; i < initialLength; i++)
        {
            Cells.Add(CreateCell(new(position.X, position.Y + i), flipColors: i == 0, Facing));
        }
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

    public bool CollidedWithSelf()
        => Cells.Count != Cells.Select(c => c.Position).Distinct().Count();

    public bool IsOccupyingPosition(Position position, bool ignoreTail = true)
        => Cells.Any(c => c.Position == position && (!ignoreTail || position != Tail.Position));

    public PlayerCell CreateCell(Position position)
            => new(position, BackgroundColor, SpriteColor, Sprite, Direction.North);

    public PlayerCell CreateCell(Position position, bool flipColors, Direction? facing = null)
    {
        if (!flipColors && facing == null) return CreateCell(position);
        var backgroundColor = BackgroundColor;
        var spriteColor = SpriteColor;

        if (flipColors)
        {
            backgroundColor = SpriteColor;
            spriteColor = BackgroundColor;
        }

        facing ??= Facing;

        return new PlayerCell(position, backgroundColor, spriteColor, facing.Value.GetSprite(), facing.Value);
    }

    public void Reverse()
    {
        Face(Tail.Facing.GetOpposite());
        Cells.Reverse();
        for (int i = 0; i < Cells.Count; i++)
        {
            var oldCell = Cells[i];
            var flipColors = oldCell == Head;

            var newCell = CreateCell(oldCell.Position, flipColors, oldCell.Facing.GetOpposite());
            Cells[i] = newCell;
        }
    }
}