using Snek.Core.Attributes;

namespace Snek.Core;

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