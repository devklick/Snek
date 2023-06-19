namespace Snek.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class SpriteAttribute : Attribute
{
    public char Sprite { get; }
    public SpriteAttribute(char sprite)
    {
        Sprite = sprite;
    }
}