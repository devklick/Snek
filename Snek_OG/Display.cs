using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snek_OG
{
    public class Display
    {
        public readonly int Width;
        public readonly int Height;

        public readonly int WidthMultiplier;
        public readonly int HeightMultiplier;

        public readonly int GameWidth;
        public readonly int GameHeight;

        public Display(int gameWidth, int gameHeight, int displayWidthMultiplier, int displayHeightMultiplier)
        {
            GameWidth = gameWidth;
            GameHeight = gameHeight;
            WidthMultiplier = displayWidthMultiplier;
            HeightMultiplier = displayHeightMultiplier;
            Width = GameWidth * WidthMultiplier;
            Height = GameHeight * HeightMultiplier;

            Console.WindowWidth = Width + 2;
            Console.WindowHeight = Height + 2;
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;
        }

        public void Draw(Cell cell)
        {
            ConsoleColor _bgCopy = Console.BackgroundColor;
            ConsoleColor _fgCopy = Console.ForegroundColor;

            Console.BackgroundColor = cell.BackgroundColour;
            Console.ForegroundColor = cell.ForegroundColour;

            // the grid cell may take up more than one display cell. 
            for (int r = 0; r < HeightMultiplier; r++)
            {
                for (int c = 0; c < WidthMultiplier; c++)
                {
                    Console.SetCursorPosition(
                        ((cell.Position.X * WidthMultiplier) - 1 + c),
                        ((cell.Position.Y * HeightMultiplier) - 1 + r)
                        );
                    Console.Write(cell.DisplayChar);
                }
            }

            Console.BackgroundColor = _bgCopy;
            Console.ForegroundColor = _fgCopy;
        }
    }
}
