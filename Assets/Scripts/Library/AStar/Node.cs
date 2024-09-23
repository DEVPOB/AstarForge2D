using UnityEngine;

public class Node
{
    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    public bool walkable;
    public float gCost;
    public float hCost;
    public Node parent;

    public float fCost => gCost + hCost;

    public Node(Vector2Int gridPos, Vector3 worldPos, bool walkable)
    {
        this.gridPosition = gridPos;
        this.worldPosition = worldPos;
        this.walkable = walkable;
    }
}

