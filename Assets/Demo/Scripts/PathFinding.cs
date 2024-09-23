using UnityEngine;
using System.Collections.Generic;
using A_Star_Library;

public class PathFinding : MonoBehaviour
{
    private GridManager gridManager;
    public Transform startPoint;
    public Vector3 endPoint;
    public Color pathColor = Color.green;
    public float moveSpeed = 10f;

    private List<Node> path;
    private Transform objectToMove;
    private bool isWalking = false;
    private float pathUpdateInterval = 0.5f;
    private float lastPathUpdateTime = 0;

    void Start()
    {
        objectToMove = GameObject.FindGameObjectWithTag("Player").transform;
        print(objectToMove);
        gridManager = FindObjectOfType<GridManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPoint.position = objectToMove.position;
            print(objectToMove.position);
            Vector3 mouseScreenPosition = Input.mousePosition;
            endPoint = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            MoveToTarget();
        }
    }
    void MoveToTarget(){

                if (gridManager != null && startPoint != null)
                {
                    path = PathfindingLibrary.FindPath(gridManager, startPoint.position, endPoint);

                    if (path != null && path.Count > 0 && !isWalking)
                    {
                        StartCoroutine(PathfindingLibrary.MoveAlongPath(objectToMove, path, moveSpeed, OnPathCompleted));
                        isWalking = true;
                    }
                }
            

    }
    void OnPathCompleted()
    {
        isWalking = false;
        Debug.Log("Path completed.");
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            PathfindingLibrary.DrawPathGizmos(path, pathColor);
        }
    }
}

