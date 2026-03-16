using System;
using System.Collections.Generic;
using System.Linq;

namespace PacManAI
{
    public class GridManager
    {
        private readonly bool[,] _walls;
        public int Width { get; }
        public int Height { get; }

        public GridManager(int width, int height, bool[,] walls)
        {
            Width = width;
            Height = height;
            _walls = walls;
        }

        public bool IsWalkable(Vector2Int pos)
        {
            if (pos.X < 0 || pos.X >= Width || pos.Y < 0 || pos.Y >= Height)
                return false;
            return !_walls[pos.X, pos.Y];
        }

        public List<Vector2Int> GetNeighbors(Vector2Int pos)
        {
            var neighbors = new List<Vector2Int>();
            var directions = new[]
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0)
            };

            foreach (var dir in directions)
            {
                var next = new Vector2Int(pos.X + dir.X, pos.Y + dir.Y);
                if (IsWalkable(next))
                    neighbors.Add(next);
            }
            return neighbors;
        }

        public List<Vector2Int> AStar(Vector2Int start, Vector2Int goal)
        {
            var openSet = new PriorityQueue<Vector2Int, int>();
            openSet.Enqueue(start, 0);

            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };
            var fScore = new Dictionary<Vector2Int, int> { [start] = Heuristic(start, goal) };

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    int tentativeGScore = gScore[current] + 1;
                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + Heuristic(neighbor, goal);
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
            return null;
        }

        private int Heuristic(Vector2Int a, Vector2Int b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            var path = new List<Vector2Int> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }
}
