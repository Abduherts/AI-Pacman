using UnityEngine;
using System.Collections.Generic;

public static class GhostEvaluator
{
    // High positive = Good for Ghost (Capture Pac-Man)
    // Low negative = Bad for Ghost (Pac-Man escapes)

    public static float EvaluateState(Node ghostNode, Node pacmanNode, GhostType type, List<Node> pellets)
    {
        float score = 0;
         // Calculate Euclidean distance between ghost and Pac-Man
        float distanceToPacman = Vector2.Distance(ghostNode.transform.position, pacmanNode.transform.position);

        switch (type)
        {
            case GhostType.Aggressive: // Blinky
                // Minimize distance to Pac-Man
                score = -distanceToPacman;
                if (distanceToPacman < 1.0f) score += 1000f; // Capture bonus
                break;

            case GhostType.Intercept: // Pinky
                // Predict Pac-Man's movement (simplified: Target 2-4 nodes ahead)
                // Since we don't have full simulation here, we assume Pac-Man continues his current direction.
                // We minimize distance to that predicted spot.
                // For this static eval, we might not have Pac-Man's direction readily available without passing movement.
                // We'll use a simpler heuristic: Maximize clogging intersection? 
                // Or just standard aggressive for now, but slightly offset?
                // Let's rely on standard Aggressive for now but weight it differently?
                // Actually, let's try to get Pac-Man's facing direction if we can, but for now:
                score = -distanceToPacman * 1.5f; // More aggressive?
                break;

            case GhostType.Defensive: // Inky
                // Protective of an area or trying to cut off?
                // Mixed behavior: If far, aggressive. If close, wander?
                if (distanceToPacman < 5f) {
                    score = distanceToPacman; // Flee/Back off if too close (Patrol-like)
                } else {
                    score = -distanceToPacman; // Chase if far
                }
                break;
                
            case GhostType.Random: // Clyde
                // Random is usually handled by not using Minimax, or random noise.
                // We'll return 0 so Minimax choices are equal, effectively random tie-breaking.
                score = 0; 
                break;
        }

        return score;
    }
}
