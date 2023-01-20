namespace Snek.Abstract;

public abstract class StyledObject
{
    /// <summary>
    /// The color that should be used behind the sprite.
    /// </summary>
    public abstract ConsoleColor BackgroundColor { get; }

    /// <summary>
    /// The color the sprite should be drawn to the console.
    /// </summary>
    public abstract ConsoleColor SpriteColor { get; }

    /// <summary>
    /// The character that should be drawn to the console to represent the sprite.
    /// </summary>
    public abstract char Sprite { get; }

    /// <summary>
    /// Creates a <see cref="Cell"/> at the specified <paramref name="position"/> with the class-level style information.
    /// </summary>
    /// <param name="position">The position the cell should occupy.</param>
    /// <returns>The new <see cref="Cell"/> instance.</returns>
    public virtual Cell CreateCell(Position position)
        => new(position, BackgroundColor, SpriteColor, Sprite);
}