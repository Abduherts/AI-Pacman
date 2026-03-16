using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    // A* Algorithm
    public static List<Node> GetPath_AStar(Node startNode, Node targetNode, out int nodesExplored)
    {
        return GetPath(startNode, targetNode, true, out nodesExplored);
    }

    // Overload for backward compatibility / convenience (returns 0 explored if not needed)
    public static List<Node> GetPath_AStar(Node startNode, Node targetNode)
    {
        return GetPath(startNode, targetNode, true, out int _);
    }

    // Dijkstra's Algorithm
    public static List<Node> GetPath_Dijkstra(Node startNode, Node targetNode, out int nodesExplored)
    {
        return GetPath(startNode, targetNode, false, out nodesExplored);
    }

    private static List<Node> GetPath(Node startNode, Node targetNode, bool useHeuristic, out int nodesExplored)
    {
        nodesExplored = 0;
        if (startNode == null || targetNode == null) return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        // Initialize start node
        startNode.gCost = 0;
        startNode.hCost = useHeuristic ? GetHeuristic(startNode, targetNode) : 0;
        startNode.parent = null;

        while (openSet.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openSet);
            nodesExplored++; // Count this visit

            if (currentNode == targetNode)
            {
                return ReconstructPath(currentNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (Node neighbor in currentNode.neighbors)
            {
                if (closedSet.Contains(neighbor)) continue;

                float newMovementCostToNeighbor = currentNode.gCost + Vector2.Distance(currentNode.transform.position, neighbor.transform.position);
                
                // If new path to neighbor is shorter or neighbor is not in openSet
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    // Heuristic is 0 for Dijkstra
                    neighbor.hCost = useHeuristic ? GetHeuristic(neighbor, targetNode) : 0;
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // No path found
    }

    private static Node GetLowestFCostNode(List<Node> openSet)
    {
        Node lowest = openSet[0];
        for (int i = 1; i < openSet.Count; i++)
        {
            if (openSet[i].fCost < lowest.fCost)
            {
                lowest = openSet[i];
            }
        }
        return lowest;
    }

    private static float GetHeuristic(Node a, Node b)
    {
        return Mathf.Abs(a.transform.position.x - b.transform.position.x) + Mathf.Abs(a.transform.position.y - b.transform.position.y);
    }

    private static List<Node> ReconstructPath(Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;
        while (current != null)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }
}
