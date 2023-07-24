using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public List<Node> path;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    
    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreatGrid();
    }

    void CreatGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldButtonLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldButtonLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint,x , y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0) { continue; }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeX) { neighbours.Add(grid[checkX, checkY]); };
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPosition(Vector3 worldposition)
    {
        float percentX = (worldposition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldposition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        // ensure percentX, percentY in (0, 1)
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY ) * percentY);
        return grid[x, y];
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(grid != null)
        {
            Node playerNode = NodeFromWorldPosition(player.position);
            foreach( Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if(path != null && path.Contains(n))
                {
                    Gizmos.color = Color.black;
                }
                if(playerNode == n)
                {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}