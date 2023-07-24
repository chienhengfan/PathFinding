using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public Transform seeker, target;
    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currrentNode = openSet[0];
            for(int i = 0; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currrentNode.fCost || openSet[i].fCost == currrentNode.fCost && openSet[i].hCost < currrentNode.hCost)
                {
                    currrentNode = openSet[i];
                }
            }
            
            openSet.Remove(currrentNode);
            closedSet.Add(currrentNode);

            if(currrentNode == targetNode)
            {
                ReTracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currrentNode))
            {
                if(!neighbour.walkable || closedSet.Contains(neighbour)) { continue; }

                int newMovementCostToNeighbour = currrentNode.gCost + GetDistance(currrentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parrent = currrentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }       
    }

    void ReTracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parrent;
        }

        path.Reverse();
        grid.path = path;
    }
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(dstX > dstY)
        {
            return 14 * dstX + 10 * (dstX - dstY);
        }
        return 14 * dstY + 10 * (dstY - dstX);
    }
}