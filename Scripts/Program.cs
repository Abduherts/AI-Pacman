using System;
using System.Collections.Generic;

namespace PacManAI
{
    class Program
    {
        static void Main(string[] args)
        {
            bool[,] walls = new bool[10, 10];
            GridManager grid = new GridManager(10, 10, walls);
            
            GameState state = new GameState();
            state.PacmanPosition = new Vector2Int(1, 1);
            state.GhostPositions.Add(new Vector2Int(8, 8));
            state.RemainingPellets.Add(new Vector2Int(2, 2));

            PacmanAgent pacman = new PacmanAgent(grid);
            Vector2Int nextMove = pacman.DecideMove(state);

            Console.WriteLine($"Pacman at {state.PacmanPosition.X},{state.PacmanPosition.Y}");
            Console.WriteLine($"Next move: {nextMove.X},{nextMove.Y}");
            
            if (nextMove == new Vector2Int(1, 2) || nextMove == new Vector2Int(2, 1))
            {
                Console.WriteLine("Test Passed: Pacman moved towards pellet.");
            }
            else
            {
                Console.WriteLine("Test Failed: Pacman did not move as expected.");
            }
        }
    }
}
