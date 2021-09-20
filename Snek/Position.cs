using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snek
{
    public class Position
    {
        /// <summary>
        /// The position along the X (horizontal) axis.
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// The position along the y (verticle) axis.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Default value to be used for the Position class. Note, this position is invalid as far as the Grid is concerned. Any X or Y value less than zero is invalid.
        /// </summary>
        public static Position Default = new Position( 0, 0 );

        public Position( int x, int y )
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gives the ability to cast a Cell to a Position.
        /// </summary>
        /// <param name="cell">The Cell instance to be casted to a Position</param>
        public static explicit operator Position( Cell cell )
        {
            return new Position( cell.Position.X, cell.Position.Y );
        }

        /// <summary>
        /// Gives the ability to perform a comparison between the current instance of the Position class with another.
        /// </summary>
        /// <param name="position">The instance of Position to compaire against the current instance</param>
        /// <returns>If the X and Y properties of the current instance of the Position class contain the same value as those on the Position specified for comparison, returns true. Otherwise false</returns>
        public bool Equals( Position position )
        {
            return position != null && position.X == X && position.Y == Y;
        }

        /// <summary>
        /// Gives the ability to compare the current instance of the Position class with another object.
        /// </summary>
        /// <param name="obj">The object to compare against the current Position instance</param>
        /// <returns>If the other object being compaired is also a Position, and has the same X and Y values, the method will return true, otherwise false</returns>
        public override bool Equals( Object obj )
        {
            if ( !( obj is Position ) )
                return false;

            Position position = (Position)obj;
            return X == position.X & Y == position.Y;
        }

        /// <summary>
        /// Gives the ability to perform a comparison between the current instance of the Position class with another, using the == operator.
        /// </summary>
        /// <param name="left">The instance of the Position class to the left of the comparison operator</param>
        /// <param name="right">The instance of the Position class to the right of the comparison operator</param>
        /// <returns>If the X and Y properties of the current instance of the Position class contain the same value as those on the Position specified for comparison, returns true. Otherwise false</returns>
        public static bool operator ==( Position left, Position right )
        {
            return EqualityComparer<Position>.Default.Equals( left, right );
        }

        /// <summary>
        /// Gives the ability to perform a comparison between the current instance of the Position class with another, using the != operator.
        /// </summary>
        /// <param name="left">The instance of the Position class to the left of the comparison operator</param>
        /// <param name="right">The instance of the Position class to the right of the comparison operator</param>
        /// <returns>If the X and Y properties of the current instance of the Position class do not contain the same value as those on the Position specified for comparison, returns true. Otherwise false</returns>
        public static bool operator !=( Position left, Position right )
        {
            return !( left == right );
        }

        /// <summary>
        /// Gives the ability to perform efficient insertions and lookups within collections that are based on a hash table.
        /// </summary>
        /// <returns>Returns an integer representation of the current Position instance</returns>
        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}
