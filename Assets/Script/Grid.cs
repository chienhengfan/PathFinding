using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public TerrainType[] walkableRegions;
    public LayerMask walkableMask;
    private Dictionary<int, int> walkableRegionDictionary = new Dictionary<int, int>();

    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public int MaxSize => gridSizeX * gridSizeY;

    // At PathFinding 15, we get Grid component,thus we should CreateGrid() at Awake
    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach(TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            // change binary number to integer (0010 0000 0000 = 2 ^ 9 >> 9)  
            walkableRegionDictionary.Add((int)Mathf.Log(region.terrainMask, 2), region.terrainPenalty);
        }
        CreatGrid();
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
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
                int movementPenalty = 0;

                //raycast to add terrainpenalty
                if (walkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                        walkableRegionDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }
                grid[x, y] = new Node(walkable, worldPoint,x , y, movementPenalty);
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


            if (grid != null && displayGridGizmos)
            {
                Node playerNode = NodeFromWorldPosition(player.position);
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }

    }
}
