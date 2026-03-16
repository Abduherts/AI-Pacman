using System;
using System.Collections.Generic;
using System.Linq;

namespace PacManAI
{
    public enum GhostType
    {
        Aggressive,
        Interceptor,
        Defensive
    }

    public class GhostAgent
    {
        private readonly GridManager _grid;
        private readonly GhostType _type;

        public GhostAgent(GridManager grid, GhostType type)
        {
            _grid = grid;
            _type = type;
        }

        public Vector2Int DecideMove(GameState state, int ghostIndex)
        {
            Vector2Int currentPos = state.GhostPositions[ghostIndex];
            Vector2Int target = GetTarget(state);

            var path = _grid.AStar(currentPos, target);
            if (path != null && path.Count > 1)
                return path[1];

            return currentPos;
        }

        private Vector2Int GetTarget(GameState state)
        {
            switch (_type)
            {
                case GhostType.Aggressive:
                    return state.PacmanPosition;
                case GhostType.Interceptor:
                    return new Vector2Int(state.PacmanPosition.X + 2, state.PacmanPosition.Y);
                case GhostType.Defensive:
                    return state.PacmanPosition;
                default:
                    return state.PacmanPosition;
            }
        }
    }
}
