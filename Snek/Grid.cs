using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snek
{
    class Grid
    {
        public int Width;
        public int Height;

        public List<Cell> Cells = new List<Cell>();
        public List<Cell> PlayerCells = new List<Cell>();
        public Cell GameObjectCell;

        private Random Rnd = new Random();

        public List<Position> AvailiblePositions
        {
            get
            {
                //return Cells.Where( c => PlayerCells.Any( pc => 
                //!( pc.Position.X == c.Position.X && pc.Position.Y == c.Position.Y )
                //&& !( pc.Position.X == GameObjectCell.Position.X && pc.Position.Y == GameObjectCell.Position.Y ) )).ToList().Select(c => c.Position).ToList();
                return Cells.Where( c => PlayerCells.Any( pc => pc.Position != c.Position && ( GameObjectCell == null || GameObjectCell != null && GameObjectCell.Position != c.Position ) ) ).ToList().Select( c => c.Position ).ToList();
            }
        }


        public Grid(int width, int height)
        {
            Width = width;
            Height = height;

            for (int x = 1; x <= width; x++ )
            {
                for ( int y = 1; y <= width; y++ )
                {
                    Cells.Add( new Cell(x, y) );
                }
            }
        }

        public Position GetRandomAvailiblePosition()
        {
            var availiblePositions = AvailiblePositions;
            return availiblePositions[Rnd.Next( 0, availiblePositions.Count() - 1 )];
        }
    }
}
