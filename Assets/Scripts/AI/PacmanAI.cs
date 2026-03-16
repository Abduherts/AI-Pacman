using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Pac-Man AI using configurable Pathfinding (A* or Dijkstra) for intelligent navigation.
/// - Uses A* or Dijkstra to find optimal path to nearest pellet
/// - Evaluates threat levels from ghosts
/// - Falls back to random direction when needed
/// </summary>
[RequireComponent(typeof(Pacman))]
public class PacmanAI : MonoBehaviour
{
    public enum PathAlgorithm { AStar, Dijkstra }

    private Pacman pacman;
    private Movement movement;
    private GridManager grid;
    private bool isInitialized = false;
    
    // Stuck detection
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private const float STUCK_THRESHOLD = 0.5f;
    
    // AI Config
    [Header("AI Configuration")]
    public PathAlgorithm algorithm = PathAlgorithm.AStar;
    public float dangerRadius = 5.0f;      // Distance to consider ghost a threat
    public float threatThreshold = 0.5f;   // Threat score threshold for fleeing

    private void Awake()
    {
        pacman = GetComponent<Pacman>();
        movement = GetComponent<Movement>();
    }

    private void Start()
    {
        grid = GridManager.Instance;
        lastPosition = transform.position;
        
        if (pacman.controlType == ControlType.HeuristicAI)
        {
            StartCoroutine(InitialMovement());
        }
    }

    private void Update()
    {
        if (!isInitialized || pacman.controlType != ControlType.HeuristicAI) return;
        
        // Stuck detection
        float distance = Vector3.Distance(transform.position, lastPosition);
        if (distance < 0.01f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= STUCK_THRESHOLD)
            {
                RecoverFromStuck();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
            lastPosition = transform.position;
        }
    }

    private void RecoverFromStuck()
    {
        if (grid == null) return;
        Node nearestNode = grid.GetClosestNode(transform.position);
        if (nearestNode == null || nearestNode.availableDirections.Count == 0) return;
        
        transform.position = nearestNode.transform.position;
        Vector2 newDir = nearestNode.availableDirections[Random.Range(0, nearestNode.availableDirections.Count)];
        Debug.Log($"PacmanAI: Stuck recovery - direction {newDir}");
        movement.SetDirection(newDir, true);
    }

    public void OnPacmanReset()
    {
        isInitialized = false;
        stuckTimer = 0f;
        StopAllCoroutines();
        if (pacman.controlType == ControlType.HeuristicAI)
            StartCoroutine(InitialMovement());
    }

    private System.Collections.IEnumerator InitialMovement()
    {
        yield return null;
        yield return null;
        yield return new WaitForFixedUpdate();
        
        if (grid == null) grid = GridManager.Instance;
        if (grid == null) yield break;
        
        Node startNode = grid.GetClosestNode(transform.position);
        if (startNode == null || startNode.availableDirections.Count == 0) yield break;

        transform.position = startNode.transform.position;
        lastPosition = transform.position;
        
        // Start with LEFT (classic Pac-Man), fallback to first available
        Vector2 startDir = startNode.availableDirections.Contains(Vector2.left) ? Vector2.left : startNode.availableDirections[0];
        Debug.Log($"PacmanAI: Starting with direction {startDir}");
        movement.SetDirection(startDir, true);
        isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        if (node == null || !isInitialized) return;
        if (pacman.controlType != ControlType.HeuristicAI) return;
        if (grid == null) grid = GridManager.Instance;

        transform.position = node.transform.position;
        lastPosition = transform.position;
        stuckTimer = 0f;

        DecideAction(node);
    }

    /// <summary>
    /// Main AI decision loop using threat evaluation and Pathfinding.
    /// </summary>
    private void DecideAction(Node currentNode)
    {
        // 1. Calculate threat score from ghosts
        List<Ghost> ghosts = new List<Ghost>(FindObjectsByType<Ghost>(FindObjectsSortMode.None));
        float threatScore = CalculateThreatScore(ghosts);
        bool isThreatened = threatScore > threatThreshold;

        // 2. Get valid directions (avoid reverse unless dead-end)
        List<Vector2> validDirs = GetValidDirections(currentNode);
        if (validDirs.Count == 0)
        {
            // Dead-end: allow any direction
            if (currentNode.availableDirections.Count > 0)
                validDirs.Add(currentNode.availableDirections[0]);
            else
                return;
        }

        Vector2 chosenDir;

        if (isThreatened)
        {
            // FLEE: Use direction that maximizes distance from threats
            chosenDir = GetFleeDirection(currentNode, validDirs, ghosts);
        }
        else
        {
            // CHASE: Use Pathfinding to path to nearest pellet
            chosenDir = GetChaseDirection(currentNode, validDirs);
        }

        movement.SetDirection(chosenDir, true);
    }

    /// <summary>
    /// Calculate threat score: Sum(1 / (distance + 1)) for non-frightened ghosts.
    /// Higher score = more danger.
    /// </summary>
    private float CalculateThreatScore(List<Ghost> ghosts)
    {
        float score = 0f;
        foreach (Ghost g in ghosts)
        {
            if (g.frightened.enabled) continue;
            float d = Vector2.Distance(transform.position, g.transform.position);
            score += 1.0f / (d + 1.0f);
        }
        return score;
    }

    /// <summary>
    /// Get directions excluding reverse (unless it's the only option).
    /// </summary>
    private List<Vector2> GetValidDirections(Node node)
    {
        List<Vector2> valid = new List<Vector2>();
        Vector2 reverse = -movement.direction;

        foreach (Vector2 dir in node.availableDirections)
        {
            if (dir == reverse && node.availableDirections.Count > 1)
                continue;
            valid.Add(dir);
        }
        return valid;
    }

    /// <summary>
    /// FLEE: Pick direction that moves away from ghosts.
    /// </summary>
    private Vector2 GetFleeDirection(Node currentNode, List<Vector2> validDirs, List<Ghost> ghosts)
    {
        Vector2 bestDir = validDirs[0];
        float maxSafetyScore = float.MinValue;

        foreach (Vector2 dir in validDirs)
        {
            Vector2 futurePos = (Vector2)currentNode.transform.position + dir;
            float safetyScore = 0f;
            
            foreach (Ghost g in ghosts)
            {
                if (g.frightened.enabled) continue;
                float d = Vector2.Distance(futurePos, g.transform.position);
                if (d < 1.0f) safetyScore -= 1000f; // Too close = bad
                else safetyScore += d;
            }

            if (safetyScore > maxSafetyScore)
            {
                maxSafetyScore = safetyScore;
                bestDir = dir;
            }
        }
        return bestDir;
    }

    /// <summary>
    /// CHASE: Use configured Pathfinding (A* or Dijkstra) to find optimal path to nearest pellet.
    /// Returns the direction of the first step in the path.
    /// </summary>
    private Vector2 GetChaseDirection(Node currentNode, List<Vector2> validDirs)
    {
        Transform targetPellet = GetBestPellet(currentNode);
        
        if (targetPellet == null)
        {
            // No pellets - pick random valid direction
            return validDirs[Random.Range(0, validDirs.Count)];
        }

        // Get target node for the pellet
        Node targetNode = grid.GetClosestNode(targetPellet.position);
        if (targetNode == null || targetNode == currentNode)
        {
            return validDirs[Random.Range(0, validDirs.Count)];
        }

        // Pathfinding: Choose algorithm
        List<Node> path;
        int nodesExplored = 0;
        
        if (algorithm == PathAlgorithm.AStar)
        {
            path = Pathfinding.GetPath_AStar(currentNode, targetNode, out nodesExplored);
        }
        else // Dijkstra
        {
            path = Pathfinding.GetPath_Dijkstra(currentNode, targetNode, out nodesExplored);
        }
        
        if (PerformanceMetrics.Instance != null)
        {
            // Assuming PerformanceMetrics has a generic log or we reuse the ghost one, 
            // but normally we'd log Pacman efficiency. 
            // For now, let's keep it simple or log if needed.
        }
        
        if (path != null && path.Count > 1)
        {
            // Calculate direction to next node in path
            Vector2 dir = (path[1].transform.position - currentNode.transform.position).normalized;
            
            // Convert to cardinal direction
            Vector2 cardinalDir;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                cardinalDir = new Vector2(Mathf.Sign(dir.x), 0);
            else
                cardinalDir = new Vector2(0, Mathf.Sign(dir.y));

            // Verify this direction is valid
            if (validDirs.Contains(cardinalDir))
                return cardinalDir;
        }

        // Fallback: use simple distance-based direction
        return GetChaseDirectionSimple(currentNode, validDirs, targetPellet);
    }

    /// <summary>
    /// Simple chase: pick direction that minimizes distance to target.
    /// </summary>
    private Vector2 GetChaseDirectionSimple(Node currentNode, List<Vector2> validDirs, Transform target)
    {
        Vector2 bestDir = validDirs[0];
        float minDist = float.MaxValue;

        foreach (Vector2 dir in validDirs)
        {
            Vector2 futurePos = (Vector2)currentNode.transform.position + dir;
            float d = Vector2.Distance(futurePos, target.position);
            if (d < minDist)
            {
                minDist = d;
                bestDir = dir;
            }
        }
        return bestDir;
    }

    /// <summary>
    /// Find best pellet to chase. Prioritizes power pellets and nearby pellets.
    /// </summary>
    private Transform GetBestPellet(Node currentNode)
    {
        if (GameManager.Instance == null || GameManager.Instance.pellets == null)
            return null;

        Transform best = null;
        float bestScore = float.MinValue;

        foreach (Transform pellet in GameManager.Instance.pellets)
        {
            if (pellet == null || !pellet.gameObject.activeSelf) continue;

            float distance = Vector2.Distance(transform.position, pellet.position);
            float score = -distance; // Closer = higher score

            // Bonus for power pellets
            if (pellet.GetComponent<PowerPellet>() != null)
                score += 50.0f;

            if (score > bestScore)
            {
                bestScore = score;
                best = pellet;
            }
        }
        return best;
    }
}
