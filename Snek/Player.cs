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

        facing ??= Facing;

        var backgroundColor = BackgroundColor;
        var spriteColor = SpriteColor;

        if (flipColors)
        {
            backgroundColor = SpriteColor;
            spriteColor = BackgroundColor;
        }

        return new PlayerCell(position, backgroundColor, spriteColor, facing.Value.GetSprite(), facing.Value);
    }

    public void Reverse()
    {
        Face(Facing.GetOpposite());
        Cells.Reverse();

        for (int i = 0; i < Cells.Count; i++)
        {
            var oldCell = Cells[i];

            // While the head has the same colors as the body, those colors are inverted. 
            // If the old cell is the head, we need to invert the colors again, 
            // as this cell will now form the tail (which is part of the body)
            var flipColors = oldCell == Head;

            // When the player is reversed, their old tail becomes their new head, 
            // and their new head must face the opposite direction of their old head.
            var facing = oldCell == Head ? Facing : oldCell.Facing.GetOpposite();

            var newCell = CreateCell(oldCell.Position, flipColors, facing);
            Cells[i] = newCell;
        }
    }
}