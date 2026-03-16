using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private LayerMask nodeLayer;

    private List<Node> nodes = new List<Node>();

    public Node GetClosestNode(Vector2 position)
    {
        Node closest = null;
        float minDistance = float.MaxValue;

        foreach (Node node in nodes)
        {
            float d = Vector2.Distance(node.transform.position, position);
            if (d < minDistance)
            {
                minDistance = d;
                closest = node;
            }
        }
        return closest;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Find all nodes in the scene
        nodes.AddRange(FindObjectsByType<Node>(FindObjectsSortMode.None));
    }

    private void Start() 
    {
        // Build graph in Start to ensure all Nodes have initialized their availableDirections
        BuildGraph();
    }

    private void BuildGraph()
    {
        foreach (var node in nodes)
        {
            ConnectNeighbors(node);
        }
    }

    private void ConnectNeighbors(Node node)
    {
        node.neighbors.Clear();

        foreach (Vector2 direction in node.availableDirections)
        {
            // Raycast to find the next node in this direction
            // We ignore the current node by starting slighty off-center or filtering
            RaycastHit2D[] hits = Physics2D.RaycastAll(node.transform.position, direction, float.MaxValue, nodeLayer);
            
            float minDistance = float.MaxValue;
            Node closestNode = null;

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == node.gameObject) continue;
                
                float d = Vector2.Distance(node.transform.position, hit.transform.position);
                
                // We only care about the CLOSEST node in this direction
                if (d < minDistance)
                {
                    minDistance = d;
                    closestNode = hit.collider.GetComponent<Node>();
                }
            }

            if (closestNode != null)
            {
                node.neighbors.Add(closestNode);
                // Debug.DrawLine(node.transform.position, closestNode.transform.position, Color.green, 100f);
            }
        }
    }
}
