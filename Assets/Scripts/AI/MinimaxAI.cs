using System.Collections.Generic;
using UnityEngine;

public static class MinimaxAI
{
    public static Node GetBestMove(Node ghostNode, Node pacmanNode, GhostType type, int depth)
    {
        // Simple Minimax: We only simulate the Ghost's move, assuming Pac-Man "plays his best" (moves away).
        // For full Minimax we'd need to simulate Pac-Man's turn too.
        // Step 1: Max (Ghost Turn)
        
        float bestVal = float.MinValue;
        Node bestMove = null;
        
        // Ghost can move to any neighbor
        foreach (Node neighbor in ghostNode.neighbors)
        {
            // Simulate moving there
            float val = Minimax(neighbor, pacmanNode, depth - 1, false, type, float.MinValue, float.MaxValue);
            
            if (val > bestVal)
            {
                bestVal = val;
                bestMove = neighbor;
            }
        }
        
        return bestMove;
    }

    private static float Minimax(Node ghostNode, Node pacmanNode, int depth, bool isMaximizing, GhostType type, float alpha, float beta)
    {
        // Leaf node or Terminal
        if (depth == 0 || ghostNode == pacmanNode)
        {
            return GhostEvaluator.EvaluateState(ghostNode, pacmanNode, type, null);
        }

        if (isMaximizing) // Ghost Turn
        {
            float maxEval = float.MinValue;
            foreach (Node neighbor in ghostNode.neighbors)
            {
                float eval = Minimax(neighbor, pacmanNode, depth - 1, false, type, alpha, beta);
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else // Pac-Man Turn (Minimizing - he wants to maximize distance / minimize our score)
        {
            float minEval = float.MaxValue;
            // Simulate Pac-Man moving to his neighbors?
            // Note: We are approximating Pac-Man's position. In reality Pac-Man moves too.
            // If we don't simulate Pac-Man moving, we assume he stays still, which is weak.
            // Let's assume Pac-Man moves to any neighbor that INCREASES distance from Ghost.
            
            foreach (Node neighbor in pacmanNode.neighbors)
            {
                float eval = Minimax(ghostNode, neighbor, depth - 1, true, type, alpha, beta);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }
}
