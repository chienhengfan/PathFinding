using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node:IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;
    public int gridX;
    public int gridY;
    public int movementPenalty;

    public Node parrent;

    private int heapIndex;
    public int HeapIndex { get => heapIndex; set { heapIndex = value; } }
    public int fCost { get { return gCost + hCost; } }
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        compare = (compare == 0) ? hCost.CompareTo(nodeToCompare.hCost) : compare;

        //fcost larger, priority smaller 
        return -compare;
    }

}
