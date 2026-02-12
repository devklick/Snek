namespace Snek.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class SpriteAttribute(char sprite) : Attribute
{
    public char Sprite { get; } = sprite;
}