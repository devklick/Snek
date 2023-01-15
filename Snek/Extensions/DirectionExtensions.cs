using System.Reflection;
using Snek.Attributes;

namespace Snek.Extensions;

public static class DirectionExtensions
{
    /// <summary>
    /// A cached map to access the sprite associated with each <see cref="Direction"/>.
    /// </summary>
    private static readonly Dictionary<Direction, char> _sprites = GetDirectionSprites();

    /// <summary>
    /// Determines whether or not the two specified parameters, <paramref name="a"/> and <paramref name="b"/>, are opposite each other.
    /// </summary>
    /// <param name="a">The fist direction</param>
    /// <param name="b">The second direction</param>
    /// <returns>`true` if they are opposite each other, otherwise `false`</returns>
    public static bool IsOpposite(this Direction a, Direction b)
        => (a == Direction.North && b == Direction.South)
            || (a == Direction.South && b == Direction.North)
            || (a == Direction.East && b == Direction.West)
            || (a == Direction.West && b == Direction.East);

    /// <summary>
    /// Gets the sprite associated with the specified <paramref name="direction"/>.
    /// </summary>
    /// <param name="direction">The <see cref="Direction"/> whose sprite should be returned.</param>
    /// <returns>The sprite associated with the specified direction.</returns>
    public static char GetSprite(this Direction direction)
        => _sprites[direction];

    /// <summary>
    /// Builds a dictionary where the key is a <see cref="Direction"/> and the value is the sprite associated with it.
    /// </summary>
    private static Dictionary<Direction, char> GetDirectionSprites()
    {
        var map = new Dictionary<Direction, char>();
        var type = typeof(Direction);
        foreach (var enumValue in Enum.GetValues(type))
        {
            var member = type.GetField(enumValue.ToString()!);
            if (member == null) continue;
            var attribute = member.GetCustomAttribute<SpriteAttribute>(false);
            if (attribute == null) continue;

            map.Add((Direction)enumValue, attribute.Sprite);
        }
        return map;
    }
}