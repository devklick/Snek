using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snek_OG
{
    class Engine
    {
        private Player Player;
        private GameObject GameObject;
        public Grid Grid;
        public Display Display;

        public bool GameOver = false;

        public int FPS = 240;
        public int TimeDelay { get { return (int)(((float)60 / FPS) * 1000); } }

        public int Score = 0;

        public Engine(int gameWidth, int gameHeight)
        {
            Grid = new Grid(gameWidth, gameHeight);
            Display = new Display(gameWidth, gameHeight, 2, 1);

            Player = new Player(this);
            GameObject = new GameObject(this);
        }

        public void Initialise()
        {
            Player.Initialise();
            GameObject.Initialise();
        }

        public void Run()
        {
            ConsoleKeyInfo keyInfo;
            bool objectEaten = false;

            while (!GameOver)
            {
                System.Threading.Thread.Sleep(TimeDelay);

                if (Console.KeyAvailable)
                {
                    keyInfo = Console.ReadKey(true);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (Player.TravelDirection != Direction.South)
                                Player.TravelDirection = Direction.North;
                            break;
                        case ConsoleKey.DownArrow:
                            if (Player.TravelDirection != Direction.North)
                                Player.TravelDirection = Direction.South;
                            break;
                        case ConsoleKey.LeftArrow:
                            if (Player.TravelDirection != Direction.East)
                                Player.TravelDirection = Direction.West;
                            break;
                        case ConsoleKey.RightArrow:
                            if (Player.TravelDirection != Direction.West)
                                Player.TravelDirection = Direction.East;
                            break;
                    }

                }

                // if the next position of the players head, based on the current position and the travel direction, 
                // collides with the bounderies or the existing cells on the snek, its game over. Otherwise, move th player
                Position nextHeadPosition = Player.NextHeadPosition();
                if (nextHeadPosition.X <= 0 || nextHeadPosition.X > Grid.Width
                    || nextHeadPosition.Y <= 0 || nextHeadPosition.Y > Grid.Height
                    || Player.Cells.Any(c => c.Position.X == nextHeadPosition.X && c.Position.Y == nextHeadPosition.Y))
                {
                    GameOver = true;
                }
                else
                {
                    Player.Move();


                    if (Player.Head.Position.X == GameObject.Cell.Position.X && Player.Head.Position.Y == GameObject.Cell.Position.Y)
                    {
                        Player.AddToTail();
                        GameObject.GenerateNew();

                        Score += 10;
                        FPS += 20;
                    }
                }



                // destroy the current game object & generate a new one

            }
        }
    }
}
