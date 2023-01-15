using Snek.Abstract;

namespace Snek;

/// <summary>
/// The object that the player is trying to target and consume.
/// </summary>
public class Enemy : StyledObject
{
    /// <summary>
    /// The cell on the grid that he enemy occupies.
    /// </summary>
    public Cell Cell { get; }
    public override ConsoleColor BackgroundColor => ConsoleColor.Cyan;
    public override ConsoleColor SpriteColor => ConsoleColor.DarkCyan;
    public override char Sprite => '#';

    public Enemy(Position position)
    {
        Cell = new(position.X, position.Y, BackgroundColor, SpriteColor, Sprite);
    }
}