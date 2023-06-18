using Snek.Interfaces;

namespace Snek.Entities;

/// <summary>
/// The object that the player is trying to target and consume.
/// </summary>
public class Enemy : IStyled<Cell>
{
    /// <summary>
    /// The cell on the grid that he enemy occupies.
    /// </summary>
    public Cell Cell { get; }
    public ConsoleColor BackgroundColor => ConsoleColor.DarkGreen;
    public ConsoleColor SpriteColor => ConsoleColor.Green;
    public char Sprite => 'Ж';

    public Enemy(Position position)
    {
        Cell = new(position, BackgroundColor, SpriteColor, Sprite);
    }

    public Cell CreateCell(Position position)
        => new(position, BackgroundColor, SpriteColor, Sprite);

    public override string ToString()
        => $"Enemy at position {Cell.Position}";
}