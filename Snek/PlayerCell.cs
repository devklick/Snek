namespace Snek;

public class PlayerCell : Cell
{
    public Direction Facing { get; set; }
    public PlayerCell(Position position) : base(position)
    {
    }

    public PlayerCell(int x, int y)
        : base(x, y)
    {
        Facing = Direction.North;
    }

    public PlayerCell(Position position, ConsoleColor backgroundColor, ConsoleColor foregroundColor, Direction facing)
        : base(position, backgroundColor, foregroundColor)
    {
        Facing = facing;
    }

    public PlayerCell(int x, int y, ConsoleColor backgroundColor, ConsoleColor foregroundColor, Direction facing)
        : base(x, y, backgroundColor, foregroundColor)
    {
        Facing = facing;
    }

    public PlayerCell(Position position, ConsoleColor backgroundColor, ConsoleColor foregroundColor, char sprite, Direction facing)
        : base(position, backgroundColor, foregroundColor, sprite)
    {
        Facing = facing;
    }

    public PlayerCell(int x, int y, ConsoleColor backgroundColor, ConsoleColor foregroundColor, char sprite, Direction facing)
        : base(x, y, backgroundColor, foregroundColor, sprite)
    {
        Facing = facing;
    }

    public override string ToString()
        => $"{Position}:{Facing}";
}