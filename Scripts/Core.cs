using System;
using System.Collections.Generic;

namespace PacManAI
{
    public struct Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj) => obj is Vector2Int other && X == other.X && Y == other.Y;
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static bool operator ==(Vector2Int a, Vector2Int b) => a.Equals(b);
        public static bool operator !=(Vector2Int a, Vector2Int b) => !a.Equals(b);
    }

    public enum GameStatePhase
    {
        Normal,
        Frightened,
        Scatter,
        Chase
    }

    public class GameState
    {
        public Vector2Int PacmanPosition { get; set; }
        public List<Vector2Int> GhostPositions { get; set; }
        public HashSet<Vector2Int> RemainingPellets { get; set; }
        public HashSet<Vector2Int> PowerPellets { get; set; }
        public GameStatePhase CurrentPhase { get; set; }
        public float FrightenedTimeRemaining { get; set; }
        public int Score { get; set; }
        public int Lives { get; set; }

        public GameState()
        {
            GhostPositions = new List<Vector2Int>();
            RemainingPellets = new HashSet<Vector2Int>();
            PowerPellets = new HashSet<Vector2Int>();
            Lives = 3;
            Score = 0;
            CurrentPhase = GameStatePhase.Normal;
        }

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
