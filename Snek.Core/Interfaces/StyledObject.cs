namespace Snek.Core.Interfaces;

public interface IStyled<TCell> where TCell : Cell
{
    /// <summary>
    /// The color that should be used behind the sprite.
    /// </summary>
    ConsoleColor BackgroundColor { get; }

    /// <summary>
    /// The color the sprite should be drawn to the console.
    /// </summary>
    ConsoleColor SpriteColor { get; }

    /// <summary>
    /// The character that should be drawn to the console to represent the sprite.
    /// </summary>
    char Sprite { get; }

    /// <summary>
    /// Creates a <see cref="Cell"/> at the specified <paramref name="position"/> with the class-level style information.
    /// </summary>
    /// <param name="position">The position the cell should occupy.</param>
    /// <returns>The new <see cref="Cell"/> instance.</returns>
    TCell CreateCell(Position position);
}