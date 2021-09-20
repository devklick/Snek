using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Snek
{
    public enum Direction { North, East, South, West }

    class Player
    {
        public Engine Engine;
        public Direction TravelDirection = Direction.West;
        public List<Cell> Cells = new List<Cell>();
        public Cell Head { get { return Cells.First(); } }
        public Cell Tail { get { return Cells.Last(); } }



        public Player(Engine engine )
        {
            Engine = engine;

            for (int i = 0; i < 5; i++ )
            {
                Cells.Add( new Cell(
                    ( Engine.Display.GameWidth / 2 ) + i,
                    engine.Display.GameHeight/ 2,
                    ConsoleColor.White,
                    ConsoleColor.Black
                    ) );
            }

            UpdatePlayerOnGrid();

        }

        public void Initialise()
        {
            foreach(Cell cell in Cells )
            {
                Engine.Display.Draw( cell );
            }
        }

        public void Move()
        {

            Position newHeadPosition = NextHeadPosition();

            Cell newHeadCell = new Cell( newHeadPosition.X, newHeadPosition.Y, ConsoleColor.White, ConsoleColor.Black );
            Cells.Insert( 0, newHeadCell );
            Engine.Display.Draw( newHeadCell );

            Cell tail = Tail;
            Cells.Remove( tail );
            Engine.Display.Draw( new Cell( tail.Position.X, tail.Position.Y, ConsoleColor.Black, ConsoleColor.Black ));

            UpdatePlayerOnGrid();
        }

        public Position NextHeadPosition()
        {
            Position nextHeadPosition;
            switch ( TravelDirection )
            {
                case Direction.North:
                    nextHeadPosition = new Position( Head.Position.X, Head.Position.Y - 1 );
                    break;
                case Direction.East:
                    nextHeadPosition = new Position( Head.Position.X + 1, Head.Position.Y );
                    break;
                case Direction.South:
                    nextHeadPosition = new Position( Head.Position.X, Head.Position.Y + 1 );
                    break;
                case Direction.West:
                    nextHeadPosition = new Position( Head.Position.X - 1, Head.Position.Y );
                    break;
                default:
                    throw new Exception();
            }
            return nextHeadPosition;
        }

        public void AddToTail()
        {
            Cells.Insert(Cells.Count() -1, new Cell(Tail.Position.X, Tail.Position.Y, ConsoleColor.White, ConsoleColor.Black));
            UpdatePlayerOnGrid();
            Engine.Display.Draw( Tail );
        }

        public void UpdatePlayerOnGrid()
        {
            Engine.Grid.PlayerCells = Cells;
        }

    }
}
