using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snek_OG
{
    public class Cell
    {
        public Position Position;

        public ConsoleColor BackgroundColour = ConsoleColor.Black;
        public ConsoleColor ForegroundColour = ConsoleColor.Black;
        public Char DisplayChar = ' ';

        public Cell(int x, int y)
        {
            Position = new Position(x, y);
        }

        public Cell(int x, int y, ConsoleColor backgroundColour, ConsoleColor foregroundColour)
        {
            Position = new Position(x, y);
            BackgroundColour = backgroundColour;
            ForegroundColour = foregroundColour;
        }

        public Cell(int x, int y, ConsoleColor backgroundColour, ConsoleColor foregroundColour, Char displayChar)
        {
            Position = new Position(x, y);
            BackgroundColour = backgroundColour;
            ForegroundColour = foregroundColour;
            DisplayChar = displayChar;
        }
    }
}
