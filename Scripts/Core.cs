using System;
using System.Collections.Generic;

namespace PacManAI
{
    /// Represents integer grid coordinates used for logical AI simulation.
    /// This avoids floating-point precision issues from Unity world positions.
    public struct Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Checks equality between two grid positions
        public override bool Equals(object obj) =>
            obj is Vector2Int other && X == other.X && Y == other.Y;

        // Required for using Vector2Int inside HashSet or Dictionary
        public override int GetHashCode() =>
            HashCode.Combine(X, Y);

        // Equality operator overload
        public static bool operator ==(Vector2Int a, Vector2Int b) =>
            a.Equals(b);

        // Inequality operator overload
        public static bool operator !=(Vector2Int a, Vector2Int b) =>
            !a.Equals(b);
    }

    /// Represents the current behavioural phase of the game.
    /// Used by AI to determine ghost behaviour.
    public enum GameStatePhase
    {
        Normal,
        Frightened,
        Scatter,
        Chase
    }

    /// Represents a complete logical game state used for AI simulation.
    /// This model is independent from Unity objects and is used for
    /// Minimax decision-making and evaluation.
    public class GameState
    {
        // Current Pac-Man grid position
        public Vector2Int PacmanPosition { get; set; }

        // Grid positions of all ghosts
        public List<Vector2Int> GhostPositions { get; set; }

        // Remaining normal pellets in the maze
        public HashSet<Vector2Int> RemainingPellets { get; set; }

        // Power pellet positions
        public HashSet<Vector2Int> PowerPellets { get; set; }

        // Current behavioural phase of the game
        public GameStatePhase CurrentPhase { get; set; }

        // Remaining frightened time (if active)
        public float FrightenedTimeRemaining { get; set; }

        // Current score
        public int Score { get; set; }

        // Remaining player lives
        public int Lives { get; set; }

        /// Initializes default game state values.
    
        public GameState()
        {
            GhostPositions = new List<Vector2Int>();
            RemainingPellets = new HashSet<Vector2Int>();
            PowerPellets = new HashSet<Vector2Int>();
            Lives = 3;
            Score = 0;
            CurrentPhase = GameStatePhase.Normal;
        }

        
        /// Creates a deep copy of the current game state.
        /// Used for safe simulation in Minimax without modifying
        /// the real game state.
        
        public GameState Clone()
        {
            return new GameState
            {
                PacmanPosition = this.PacmanPosition,
                GhostPositions = new List<Vector2Int>(this.GhostPositions),
                RemainingPellets = new HashSet<Vector2Int>(this.RemainingPellets),
                PowerPellets = new HashSet<Vector2Int>(this.PowerPellets),
                CurrentPhase = this.CurrentPhase,
                FrightenedTimeRemaining = this.FrightenedTimeRemaining,
                Score = this.Score,
                Lives = this.Lives
            };
        }
    }
}
