using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snek
{
    class GameObject
    {
        public Engine Engine;
        public Cell Cell;

        public GameObject(Engine engine)
        {
            Engine = engine;
            GenerateNew();
            UpdateGameObjectOnGrid();
        }

        public void Initialise()
        {
            Draw();
        }

        public void GenerateNew()
        {
            Position objectStartPosition = Engine.Grid.GetRandomAvailiblePosition();
            Cell = new Cell( objectStartPosition.X, objectStartPosition.Y, ConsoleColor.Blue, ConsoleColor.Red );
            Draw();
        }

        public void Draw()
        {
            Engine.Display.Draw( Cell );
        }

        public void UpdateGameObjectOnGrid()
        {
            Engine.Grid.GameObjectCell = Cell;
        }
    }
}
