using System;
using System.Collections.Generic;
using System.Linq;

namespace PacManAI
{
    // Defines Pac-Man behavioural modes
    public enum PacmanState
    {
        ChasingPellets,
        Fleeing
    }

    public class PacmanAgent
    {
        // Distance threshold for detecting danger
        private const int DangerThreshold = 3;

        // Reference to grid system for pathfinding
        private readonly GridManager _grid;

        // Constructor injects grid dependency
        public PacmanAgent(GridManager grid)
        {
            _grid = grid;
        }

        // Main decision function for Pac-Man movement
        public Vector2Int DecideMove(GameState state)
        {
            // Find nearest ghost
            var nearestGhost = FindNearestGhost(state);

            // Calculate Manhattan distance to ghost
            int ghostDist = GetDistance(state.PacmanPosition, nearestGhost);

            // If ghost is too close and not frightened, flee
            if (ghostDist <= DangerThreshold &&
                state.CurrentPhase != GameStatePhase.Frightened)
            {
                return Flee(state, nearestGhost);
            }

            // Otherwise, chase nearest pellet
            return ChasePellet(state);
        }

        private Vector2Int ChasePellet(GameState state)
        {
            // Find closest remaining pellet
            var nearestPellet = state.RemainingPellets
                .OrderBy(p => GetDistance(state.PacmanPosition, p))
                .FirstOrDefault();

            // If no pellets left, stay in place
            if (nearestPellet == default)
                return state.PacmanPosition;

            // Use A* to compute shortest path
            var path = _grid.AStar(
                state.PacmanPosition,
                nearestPellet
            );

            // Move one step along the path
            return (path != null && path.Count > 1)
                ? path[1]
                : state.PacmanPosition;
        }

        private Vector2Int Flee(GameState state, Vector2Int ghostPos)
        {
            // Get all valid neighboring tiles
            var possibleMoves =
                _grid.GetNeighbors(state.PacmanPosition);

            // Choose move that maximizes distance from ghost
            return possibleMoves
                .OrderByDescending(
                    m => GetDistance(m, ghostPos))
                .FirstOrDefault();
        }

        private Vector2Int FindNearestGhost(GameState state)
        {
            // If no ghosts, return dummy position
            if (state.GhostPositions.Count == 0)
                return new Vector2Int(-100, -100);

            // Select ghost with minimum distance
            return state.GhostPositions
                .OrderBy(
                    g => GetDistance(
                        state.PacmanPosition, g))
                .First();
        }

        private int GetDistance(
            Vector2Int a, Vector2Int b)
        {
            // Manhattan distance calculation
            return Math.Abs(a.X - b.X)
                + Math.Abs(a.Y - b.Y);
        }
    }
}
