using Snek.Events;
using Snek.Interfaces;

namespace Snek.UI;

public class Form : IStyled<Cell>, IGrid
{
    public int Width => 20;

    public int Height => 20;

    public List<Cell> Cells => new();

    public Position Offset => Position.Default;

    public ConsoleColor BackgroundColor => ConsoleColor.Gray;

    public ConsoleColor SpriteColor => ConsoleColor.Black;

    public char Sprite => ' ';

    public event CellUpdatedEventHandler? CellUpdated;

    public Cell CreateCell(Position position)
    {
        throw new NotImplementedException();
    }
}
