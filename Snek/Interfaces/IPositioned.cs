namespace Snek.Interfaces;

public interface IPositioned
{
    /// <summary>
    /// The position that an object occupies.
    /// </summary>
    public Position Position { get; }
}