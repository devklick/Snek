namespace Snek.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class MapToDirectionAttribute(Direction direction) : Attribute
{
    public Direction Direction { get; } = direction;
}