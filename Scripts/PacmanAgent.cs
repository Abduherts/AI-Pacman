using System;
using System.Collections.Generic;
using System.Linq;

namespace PacManAI
{
    public enum PacmanState
    {
        ChasingPellets,
        Fleeing
    }

    public class PacmanAgent
    {
        private const int DangerThreshold = 3;
        private readonly GridManager _grid;

        public PacmanAgent(GridManager grid)
        {
            _grid = grid;
        }

        public Vector2Int DecideMove(GameState state)
        {
            var nearestGhost = FindNearestGhost(state);
            int ghostDist = GetDistance(state.PacmanPosition, nearestGhost);

            if (ghostDist <= DangerThreshold && state.CurrentPhase != GameStatePhase.Frightened)
            {
                return Flee(state, nearestGhost);
            }

            return ChasePellet(state);
        }

        private Vector2Int ChasePellet(GameState state)
        {
            var nearestPellet = state.RemainingPellets
                .OrderBy(p => GetDistance(state.PacmanPosition, p))
                .FirstOrDefault();

            if (nearestPellet == default) return state.PacmanPosition;

            var path = _grid.AStar(state.PacmanPosition, nearestPellet);
            return (path != null && path.Count > 1) ? path[1] : state.PacmanPosition;
        }

        private Vector2Int Flee(GameState state, Vector2Int ghostPos)
        {
            var possibleMoves = _grid.GetNeighbors(state.PacmanPosition);
            return possibleMoves
                .OrderByDescending(m => GetDistance(m, ghostPos))
                .FirstOrDefault();
        }

        private Vector2Int FindNearestGhost(GameState state)
        {
            if (state.GhostPositions.Count == 0) return new Vector2Int(-100, -100);
            return state.GhostPositions
                .OrderBy(g => GetDistance(state.PacmanPosition, g))
                .First();
        }

        private int GetDistance(Vector2Int a, Vector2Int b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
