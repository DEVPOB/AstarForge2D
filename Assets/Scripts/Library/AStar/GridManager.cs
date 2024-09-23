using UnityEngine;
using System.Collections.Generic;
using A_Star_Library;

public class GridManager : MonoBehaviour
{
    public int gridWidth, gridHeight;
    public float nodeSize;
    public float checkBox;
    public LayerMask unwalkableMask;
    public float threshold = 1f;
    private Node[,] grid;
    void Start(){
        grid = GridManagerLibrary.CreateGrid(gridWidth, gridHeight, nodeSize, checkBox, unwalkableMask, threshold, transform);
    }

    public List<Node> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition){
        return GridManagerLibrary.FindPath(grid, startWorldPosition, endWorldPosition, gridWidth, gridHeight);
    }

    void OnDrawGizmos(){
        if (grid != null){
            foreach (Node node in grid){
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSize - 0.1f));
            }
        }
    }


}
