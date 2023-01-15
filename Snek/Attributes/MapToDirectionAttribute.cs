namespace Snek.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class MapToDirectionAttribute : Attribute
{
    public Direction Direction { get; }

    public MapToDirectionAttribute(Direction direction)
    {
        Direction = direction;
    }
}