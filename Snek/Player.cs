using Snek.Extensions;
using Snek.Infrastructure;
using Snek.Interfaces;

namespace Snek;

public class Player : IStyled<PlayerCell>
{
    public ConsoleColor BackgroundColor => ConsoleColor.White;
    public ConsoleColor SpriteColor => ConsoleColor.Red;
    public char Sprite => Facing.GetSprite();
    public Direction Facing { get; private set; }
    public List<PlayerCell> Cells { get; }
    public PlayerCell Head => Cells.First();
    public PlayerCell Tail => Cells.Last();
    private readonly FileLogger _logger;

    public Player(FileLogger logger, Position midPoint, int initialLength = 5, Direction facing = Direction.North)
    {
        _logger = logger;
        Cells = new();
        Facing = facing;
        for (var i = 0; i < initialLength; i++)
        {
            var x = Facing.IsHorizontal() ? (midPoint.X / 2) + i : midPoint.X;
            var y = Facing.IsVertical() ? (midPoint.Y / 2) + i : midPoint.Y;
            Cells.Add(CreateCell(new(x, y), flipColors: i == 0, Facing));
        }
        LogSelf();
    }

    public bool CanFace(Direction direction)
    {
        var willCollideWithNext = Head.Position.GetNeighbor(direction) == Cells.ElementAt(1).Position;
        if (willCollideWithNext)
        {
            _logger.LogInfo($"Player cannot face {direction} as it would collide with the second cell");
            return false;
        }
        var result = !direction.IsOpposite(Facing);
        _logger.LogInfo($"Player facing {Facing}, {(result ? "can" : "can not")} face {direction}");
        return result;
    }

    public void Face(Direction direction)
    {
        Facing = direction;
        Head.Facing = direction;
        LogSelf();
    }

    /// <summary>
    /// The next position that the players head will occupy if they move one cell in the current direction.
    /// </summary>
    public Position NextHeadPosition()
        => Head.Position.GetNeighbor(Facing);

    public bool CollidedWithSelf()
        => Cells.Count != Cells.Select(c => c.Position).Distinct().Count();

    public bool IsOccupyingPosition(Position position, bool ignoreTail = true)
        => Cells.Any(c => c.Position == position && (!ignoreTail || position != Tail.Position));

    public PlayerCell CreateCell(Position position)
            => new(position, BackgroundColor, SpriteColor, Sprite, Direction.North);

    public PlayerCell CreateCell(Position position, bool flipColors, Direction? facing = null)
    {
        if (!flipColors && facing == null) return CreateCell(position);

        facing ??= Facing;

        var backgroundColor = BackgroundColor;
        var spriteColor = SpriteColor;

        if (flipColors)
        {
            backgroundColor = SpriteColor;
            spriteColor = BackgroundColor;
        }

        return new PlayerCell(position, backgroundColor, spriteColor, facing.Value.GetSprite(), facing.Value);
    }

    public void Reverse()
    {
        LogSelf();

        var nextHeadFacing = GetNextHeadFacingDirection();

        ReverseCells(nextHeadFacing);

        Face(nextHeadFacing);

        _logger.LogInfo("Player reversed");
        LogSelf();
    }

    public override string ToString()
        => $"Player facing {Facing}, head is {Head}, tail is {Tail}, cells are {string.Join(';', Cells.Select(c => c.ToString()))}";

    private void ReverseCells(Direction nextHeadFacing)
    {
        Cells.Reverse();

        for (int i = 0; i < Cells.Count; i++)
        {
            var oldCell = Cells[i];

            // While the head has the same colors as the body, those colors are inverted. 
            // If the old cell is the head, we need to invert the colors again, 
            // as this cell will now form the tail (which is part of the body)
            var flipColors = oldCell == Head;

            // When the player is reversed, their old tail becomes their new head, 
            // and their new head must face the opposite direction of their old head.
            var facing = oldCell == Head ? nextHeadFacing : oldCell.Facing.GetOpposite();

            var newCell = CreateCell(oldCell.Position, flipColors, facing);
            Cells[i] = newCell;
        }
    }

    private Direction GetNextHeadFacingDirection()
    {
        // If possible, the reversed snake should face the direction 
        // that is opposite to the direction it is currently facing.
        var nextHeadFacing = Facing.GetOpposite();

        // If we reverse the snake and set the head facing this direction, it's possible
        // that it'll be pointing towards the second cell, and will immediately collide with itself. 
        // In this scenario, we work out the direction the current tail (before reversing) is currently
        // traveling in, and set the new head to face the opposite direction.
        var secondLast = Cells[^2];
        var neighbor = Tail.Position.GetNeighbor(nextHeadFacing);

        if (secondLast.Position == neighbor)
        {
            var directionOfTravel = GetDirectionOfTravel(Tail.Position, secondLast.Position);
            nextHeadFacing = directionOfTravel.GetOpposite();
            var message = $"Player would instantly collide with self if facing it's natural opposite direction, {Facing.GetOpposite()}. ";
            message += $"instead face the opposite direction that the tail is currently traveling in, {nextHeadFacing}";
            _logger.LogInfo(message);
        }
        else
        {
            _logger.LogInfo($"Player can face it's natural opposite direction {nextHeadFacing}");
        }

        return nextHeadFacing;
    }
    private void LogSelf()
        => _logger.LogInfo(ToString());

    private static Direction GetDirectionOfTravel(Position first, Position second)
    {
        if (first.X > second.X) return Direction.West;
        if (first.X < second.X) return Direction.East;
        if (first.Y > second.Y) return Direction.North;
        if (first.Y < second.Y) return Direction.South;
        throw new NotImplementedException("No implementation for cells that are on the position");
    }
}