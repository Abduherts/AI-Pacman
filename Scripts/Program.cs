using System;
using System.Collections.Generic;

namespace PacManAI
{
    class Program
    {
        // Entry point of the console application
        // This runs when executing the program outside Unity
        static void Main(string[] args)
        {
            // Create a 10x10 grid with no walls (false = no wall)
            bool[,] walls = new bool[10, 10];

            // Initialize GridManager with width, height, and wall layout
            GridManager grid = new GridManager(10, 10, walls);
            
            // Create logical game state for testing
            GameState state = new GameState();

            // Set Pac-Man starting position
            state.PacmanPosition = new Vector2Int(1, 1);

            // Add a ghost far away from Pac-Man
            state.GhostPositions.Add(new Vector2Int(8, 8));

            // Add a pellet near Pac-Man
            state.RemainingPellets.Add(new Vector2Int(2, 2));

            // Create Pac-Man AI agent with grid dependency
            PacmanAgent pacman = new PacmanAgent(grid);

            // Ask AI to determine next move
            Vector2Int nextMove = pacman.DecideMove(state);

            // Print current Pac-Man position
            Console.WriteLine($"Pacman at {state.PacmanPosition.X},{state.PacmanPosition.Y}");

            // Print AI-decided next move
            Console.WriteLine($"Next move: {nextMove.X},{nextMove.Y}");
            
            // Validate if Pac-Man moves toward pellet position (2,2)
            if (nextMove == new Vector2Int(1, 2) || 
                nextMove == new Vector2Int(2, 1))
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
