﻿using Snek.Abstract;
using Snek.Interfaces;

namespace Snek;

public class Cell : StyledObject, IPositioned
{
    public Position Position { get; }
    public override ConsoleColor BackgroundColor { get; } = ConsoleColor.Black;
    public override ConsoleColor SpriteColor { get; } = ConsoleColor.Black;
    public override char Sprite { get; } = ' ';

    public Cell(int x, int y) : this(new Position(x, y))
    { }

    public Cell(Position position)
    {
        Position = position;
    }

    public Cell(int x, int y, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        : this(new Position(x, y), backgroundColor, foregroundColor)
    { }

    public Cell(Position position, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
    {
        Position = position;
        BackgroundColor = backgroundColor;
        SpriteColor = foregroundColor;
    }

    public Cell(int x, int y, ConsoleColor backgroundColor, ConsoleColor foregroundColor, char sprite)
        : this(new Position(x, y), backgroundColor, foregroundColor, sprite)
    { }

    public Cell(Position position, ConsoleColor backgroundColor, ConsoleColor foregroundColor, char sprite)
    {
        Position = position;
        BackgroundColor = backgroundColor;
        SpriteColor = foregroundColor;
        Sprite = sprite;
    }
}
