using Snek.Attributes;

namespace Snek;

public enum Direction
{
    [Sprite('↑')]
    North,

    [Sprite('→')]
    East,

    [Sprite('↓')]
    South,

    [Sprite('←')]
    West
}