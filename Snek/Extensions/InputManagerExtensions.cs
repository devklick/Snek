using System.Reflection;
using Snek.Attributes;

namespace Snek.Extensions;

public static class InputManagerExtensions
{
    /// <summary>
    /// A cached map to access the <see cref="Direction"/> associated with a given <see cref="PlayerInput"/>, if applicable. 
    /// </summary>
    /// <remarks>Note that not all <see cref="PlayerInput"/> values map to a <see cref="Direction"/>.</remarks>
    private static readonly Dictionary<PlayerInput, Direction?> _inputToDirection = GetInputToDirectionMap();

    /// <summary>
    /// Checks if the specified <paramref name="playerInput"/> can be considered a direction. 
    /// If so, true is returned and the corresponding <see cref="Direction"/> is assigned to the `out` param <paramref name="direction"/>.
    /// </summary>
    /// <param name="playerInput">The value to be checked</param>
    /// <param name="direction">The corresponding direction, is applicable</param>
    public static bool IsDirection(this PlayerInput playerInput, out Direction? direction)
        => _inputToDirection.TryGetValue(playerInput, out direction);

    /// <summary>
    /// Builds a dictionary where the key is a <see cref="PlayerInput"/> and the value is the <see cref="Direction"/> associated with it, if applicable.
    /// </summary>
    private static Dictionary<PlayerInput, Direction?> GetInputToDirectionMap()
    {
        var map = new Dictionary<PlayerInput, Direction?>();
        var type = typeof(PlayerInput);
        foreach (var enumValue in Enum.GetValues(type))
        {
            var member = type.GetField(enumValue.ToString()!);
            if (member == null) continue;
            var attribute = member.GetCustomAttribute<MapToDirectionAttribute>(false);
            if (attribute == null) continue;

            map.Add((PlayerInput)enumValue, attribute.Direction);
        }
        return map;
    }
}