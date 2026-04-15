using System;
// Provides generic collection types such as List, HashSet,
// and Dictionary for storing and managing data safely.
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
// LINQ is used to filter and search data inside collections.

namespace PacManAI
{
    // Defines different ghost behaviour types
    public enum GhostType
    {
        Aggressive,   // Directly chases Pac-Man
        Interceptor,  // Attempts to intercept ahead of Pac-Man
        Defensive     // Less aggressive behaviour
    }

    // Logical AI controller for a ghost (not a Unity MonoBehaviour)
    public class GhostAgent
    {
        // Reference to grid system used for A* pathfinding
        private readonly GridManager _grid;

        // Defines this ghost's personality type
        private readonly GhostType _type;

        // Constructor injects dependencies (grid + behaviour type)
        public GhostAgent(GridManager grid, GhostType type)
        {
            _grid = grid;
            _type = type;
        }

        // Determines the next movement position for a ghost
        public Vector2Int DecideMove(GameState state, int ghostIndex)
        {
            // Get current ghost position from game state
            Vector2Int currentPos = state.GhostPositions[ghostIndex];

            // Determine behavioural target based on ghost type
            Vector2Int target = GetTarget(state);

            // Compute shortest path using A* algorithm
            var path = _grid.AStar(currentPos, target);

            // If a valid path exists, move one step forward
            // path[0] = current position
            // path[1] = next position
            if (path != null && path.Count > 1)
                return path[1];

            // If no path found, remain in current position
            return currentPos;
        }

        // Selects target tile based on ghost personality
        private Vector2Int GetTarget(GameState state)
        {
            switch (_type)
            {
                case GhostType.Aggressive:
                    // Direct chase behaviour
                    return state.PacmanPosition;

                case GhostType.Interceptor:
                    // Simplified prediction: target 2 tiles ahead
                    return new Vector2Int(
                        state.PacmanPosition.X + 2,
                        state.PacmanPosition.Y);

                case GhostType.Defensive:
                    // Currently behaves similar to aggressive
                    return state.PacmanPosition;

                default:
                    return state.PacmanPosition;
            }
        }
    }
}
