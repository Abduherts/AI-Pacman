using UnityEngine;
using System.Collections.Generic;

public class GhostChase : GhostBehavior
{
    private List<Node> currentPath;
    
    public enum PathAlgorithm { AStar, Dijkstra }
    public PathAlgorithm algorithm = PathAlgorithm.AStar;

    private void OnDisable()
    {
        ghost.scatter.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        // Do nothing while the ghost is frightened or if disabled
        if (node != null && enabled && !ghost.frightened.enabled)
        {
            if (GridManager.Instance == null) return;

            if (GridManager.Instance == null) return;

            Node startNode = node; // Current node
            Node pacmanNode = GridManager.Instance.GetClosestNode(ghost.target.position);
            
            // Choose AI Algorithm based on Type
            if (ghost.aiType == GhostType.Random)
            {
                 if (node.availableDirections != null && node.availableDirections.Count > 0) {
                     int index = Random.Range(0, node.availableDirections.Count);
                     ghost.movement.SetDirection(node.availableDirections[index]);
                 }
                 return;
            }

            Node bestMoveNode = null;
            
            // Note: Minimax currently uses A* internally in its evaluation (GhostEvaluator potentially uses path distance)
            // or simply Node neighbors.
            // If we want to compare A* vs Dijkstra for the *Chase* path itself (if not using Minimax):
            // But we ARE using Minimax. 
            // Minimax decides "Target Node". But wait, Minimax decides the *Immediate Next Step*.
            // So A*/Dijkstra is only used if we are NOT using Minimax, OR if Minimax uses it for heuristics.
            
            // If we want to strictly compare A* vs Dijkstra movement:
            // We should bypass Minimax for strict pathfinding testing if that's the goal.
            // OR we update Minimax to use a specific distance function.
            
            // For now, let's assume we use the pathfinding for the "Path to Target" calculation 
            // used effectively when Minimax is NOT active or for Debugging.
            
            // Actually, your GhostChase uses MinimaxAI.GetBestMove.
            // The algorithm choice mainly affects how *Pathfinding* utility works.
            // To evaluate, let's switch back to pure Pathfinding for the comparison test?
            // OR update Minimax to run? 
            
            // Let's stick to the Plan: "Compare performance with Dijkstra".
            // We will use A* / Dijkstra to calculate the path to target *for the Ghost*.
            
            if (ghost.aiType == GhostType.Random) {
                 // ...
            }
            
            // For rigorous testing, let's allow disabling Minimax to test pure Pathfinding speed?
            // Or just use the Algorithm to visualize the path in Gizmos?
            
            // Let's rely on Minimax for decision, but calculate the FULL PATH using the selected algo for Metrics/Gizmos?
            // That adds overhead. 
            
            // But for now, let's just use it to generate the `currentPath` variable which helps Visuals + Metrics.
            
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int nodesExplored = 0;
            
            if (pacmanNode != null) {
                if (algorithm == PathAlgorithm.AStar)
                    currentPath = Pathfinding.GetPath_AStar(startNode, pacmanNode, out nodesExplored);
                else
                    currentPath = Pathfinding.GetPath_Dijkstra(startNode, pacmanNode, out nodesExplored); // Fallback/Test
            }
            
            if (PerformanceMetrics.Instance != null)
            {
                PerformanceMetrics.Instance.LogGhostPathEfficiency(nodesExplored);
            }
            
            // ... (Minimax execution continues)
            bestMoveNode = MinimaxAI.GetBestMove(startNode, pacmanNode, ghost.aiType, 3);
            
            sw.Stop();
            if (PerformanceMetrics.Instance != null)
            {
                PerformanceMetrics.Instance.LogGhostDecision((float)sw.Elapsed.TotalMilliseconds);
            }

            if (bestMoveNode != null)
            {
                // Move towards the best node returned by Minimax
                Vector2 direction = (bestMoveNode.transform.position - node.transform.position).normalized;
                
                // Snap to cardinal
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                    direction = new Vector2(Mathf.Sign(direction.x), 0);
                } else {
                    direction = new Vector2(0, Mathf.Sign(direction.y));
                }

                ghost.movement.SetDirection(direction);
                
                // Visualization (Path not strictly available from Minimax recursive call without extra work, 
                // so we won't draw the full path, maybe just the line to the choice)
                currentPath = new List<Node> { startNode, bestMoveNode };
            }
            else
            {
                // Fallback if Minimax fails or returns null (dead end?)
                if (node.availableDirections != null && node.availableDirections.Count > 0) {
                     int index = Random.Range(0, node.availableDirections.Count);
                     if (index >= 0 && index < node.availableDirections.Count) {
                        ghost.movement.SetDirection(node.availableDirections[index]);
                     }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i].transform.position, currentPath[i+1].transform.position);
            }
        }
    }
}
