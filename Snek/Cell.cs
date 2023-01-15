using Snek.Abstract;
using Snek.Interfaces;

namespace Snek;

public class Cell : StyledObject, IPositioned
{
    public Position Position { get; }
    public override ConsoleColor BackgroundColor { get; } = ConsoleColor.Black;
    public override ConsoleColor SpriteColor { get; } = ConsoleColor.Black;
    public override char Sprite { get; } = ' ';

    public Cell(int x, int y)
    {
        Position = new Position(x, y);
    }

    public Cell(int x, int y, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
    {
        Position = new Position(x, y);
        BackgroundColor = backgroundColor;
        SpriteColor = foregroundColor;
    }

    public Cell(int x, int y, ConsoleColor backgroundColor, ConsoleColor foregroundColor, char sprite)
    {
        Position = new Position(x, y);
        BackgroundColor = backgroundColor;
        SpriteColor = foregroundColor;
        Sprite = sprite;
    }
}
