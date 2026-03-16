using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;

public class PerformanceMetrics : MonoBehaviour
{
    public static PerformanceMetrics Instance { get; private set; }

    // Metrics
    public float survivalTime { get; private set; }
    private bool isTracking = false;

    // Latency Tracking
    private float totalPacmanDecisionTimeMs = 0;
    private int pacmanDecisionCount = 0;

    private float totalGhostDecisionTimeMs = 0;
    private int ghostDecisionCount = 0;
    
    private int totalGhostNodesExplored = 0;
    private int ghostPathfindingCount = 0;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (isTracking)
        {
            survivalTime += Time.deltaTime;
        }
    }

    public void StartTracking()
    {
        survivalTime = 0;
        totalPacmanDecisionTimeMs = 0;
        pacmanDecisionCount = 0;
        totalGhostDecisionTimeMs = 0;
        ghostDecisionCount = 0;
        totalGhostNodesExplored = 0;
        ghostPathfindingCount = 0;
        isTracking = true;
        UnityEngine.Debug.Log("Performance Tracking Started.");
    }

    public void StopTracking()
    {
        isTracking = false;
        PrintReport();
    }

    public void LogPacmanDecision(float ms)
    {
        if (!isTracking) return;
        totalPacmanDecisionTimeMs += ms;
        pacmanDecisionCount++;
    }

    public void LogGhostDecision(float ms)
    {
        if (!isTracking) return;
        totalGhostDecisionTimeMs += ms;
        ghostDecisionCount++;
    }
    
    public void LogGhostPathEfficiency(int nodesExplored)
    {
        if (!isTracking) return;
        totalGhostNodesExplored += nodesExplored;
        ghostPathfindingCount++;
    }

    private void PrintReport()
    {
        float avgPacman = pacmanDecisionCount > 0 ? (totalPacmanDecisionTimeMs / pacmanDecisionCount) : 0;
        float avgGhost = ghostDecisionCount > 0 ? (totalGhostDecisionTimeMs / ghostDecisionCount) : 0;
        float avgNodes = ghostPathfindingCount > 0 ? (float)totalGhostNodesExplored / ghostPathfindingCount : 0;

        UnityEngine.Debug.Log("=== PERFORMANCE REPORT ===");
        UnityEngine.Debug.Log($"Survival Time: {survivalTime:F2} seconds");
        UnityEngine.Debug.Log($"Pac-Man (AI) Avg Latency: {avgPacman:F4} ms ({pacmanDecisionCount} decisions)");
        UnityEngine.Debug.Log($"Ghost (Minimax) Avg Latency: {avgGhost:F4} ms ({ghostDecisionCount} decisions)");
        UnityEngine.Debug.Log($"Ghost Pathfinding Efficiency: {avgNodes:F1} nodes explored per path");
        UnityEngine.Debug.Log("==========================");
    }
}
