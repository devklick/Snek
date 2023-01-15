namespace Snek;

public struct Position
{
    /// <summary>
    /// The position along the X (horizontal) axis.
    /// </summary>
    public int X { get; }
    /// <summary>
    /// The position along the y (vertical) axis.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Default value to be used for the Position class.
    /// </summary>
    public static readonly Position Default = new(0, 0);

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }


    /// <summary>
    /// Gives the ability to perform a comparison between the current instance of the Position class with another.
    /// </summary>
    /// <param name="position">The instance of Position to compared against the current instance</param>
    /// <returns>If the X and Y properties of the current instance of the Position class contain the same value as those on the Position specified for comparison, returns true. Otherwise false</returns>
    public bool Equals(Position position)
    {
        return position.X == X && position.Y == Y;
    }

    /// <summary>
    /// Gives the ability to compare the current instance of the Position class with another object.
    /// </summary>
    /// <param name="obj">The object to compare against the current Position instance</param>
    /// <returns>If the other object being compared is also a Position, and has the same X and Y values, the method will return true, otherwise false</returns>
    public override bool Equals(object? obj)
    {
        return obj is Position position && Equals(position);
    }

    /// <summary>
    /// Gives the ability to perform a comparison between the current instance of the Position class with another, using the == operator.
    /// </summary>
    /// <param name="left">The instance of the Position class to the left of the comparison operator</param>
    /// <param name="right">The instance of the Position class to the right of the comparison operator</param>
    /// <returns>If the X and Y properties of the current instance of the Position class contain the same value as those on the Position specified for comparison, returns true. Otherwise false</returns>
    public static bool operator ==(Position left, Position right)
    {
        return EqualityComparer<Position>.Default.Equals(left, right);
    }

    /// <summary>
    /// Gives the ability to perform a comparison between the current instance of the Position class with another, using the != operator.
    /// </summary>
    /// <param name="left">The instance of the Position class to the left of the comparison operator</param>
    /// <param name="right">The instance of the Position class to the right of the comparison operator</param>
    /// <returns>If the X and Y properties of the current instance of the Position class do not contain the same value as those on the Position specified for comparison, returns true. Otherwise false</returns>
    public static bool operator !=(Position left, Position right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Gives the ability to perform efficient insertions and lookups within collections that are based on a hash table.
    /// </summary>
    /// <returns>Returns an integer representation of the current Position instance</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(X);
        hash.Add(Y);
        return hash.ToHashCode();
    }
}
