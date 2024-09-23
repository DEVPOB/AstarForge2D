
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoxColliderPlus;

namespace A_Star_Library
{
    public static class GridManagerLibrary
    {
        /// <summary>
        /// Creates the grid and returns a 2D array of Nodes.
        /// </summary>
        /// <param name="gridWidth">Width of the grid.</param>
        /// <param name="gridHeight">Height of the grid.</param>
        /// <param name="nodeSize">Size of each node.</param>
        /// <param name="CheckBox">Size of the check box for overlap detection.</param>
        /// <param name="unwalkableMask">Layer mask to determine unwalkable areas.</param>
        /// <param name="threshold">Threshold for determining small overlaps.</param>
        /// <returns>2D array of nodes representing the grid.</returns>
        public static Node[,] CreateGrid(int gridWidth, int gridHeight, float nodeSize, float CheckBox, LayerMask unwalkableMask, float threshold, Transform gridTransform)
        {
            Node[,] grid = new Node[gridWidth, gridHeight];
            Vector3 worldBottomLeft = gridTransform.position - Vector3.right * (gridWidth * nodeSize) / 2 - Vector3.up * (gridHeight * nodeSize) / 2;
            Vector2 size = new Vector2(CheckBox, CheckBox);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeSize + nodeSize) + Vector3.up * (y * nodeSize + nodeSize / 2);
                    Collider2D hit = Physics2D.OverlapBox(worldPoint, size, 0, unwalkableMask);
                    bool walkable = hit == null || !OverlapUtility.IsSmallOverlap(hit, worldPoint, size, threshold);
                    grid[x, y] = new Node(new Vector2Int(x, y), worldPoint, walkable);
                }
            }
            return grid;
        }
        

        /// <summary>
        /// Finds the path between two world positions.
        /// </summary>
        /// <param name="grid">The 2D array of nodes representing the grid.</param>
        /// <param name="startWorldPosition">The starting world position.</param>
        /// <param name="endWorldPosition">The ending world position.</param>
        /// <param name="gridWidth">Width of the grid.</param>
        /// <param name="gridHeight">Height of the grid.</param>
        /// <returns>List of nodes representing the path.</returns>
        public static List<Node> FindPath(Node[,] grid, Vector3 startWorldPosition, Vector3 endWorldPosition, int gridWidth, int gridHeight)
        {
            Node startNode = GetNodeFromWorldPosition(grid, startWorldPosition, gridWidth, gridHeight);
            Node endNode = GetNodeFromWorldPosition(grid, endWorldPosition, gridWidth, gridHeight);

            List<Node> openSet = new List<Node> { startNode };
            HashSet<Node> closedSet = new HashSet<Node>();

            foreach (Node node in grid)
            {
                node.gCost = float.MaxValue;
                node.hCost = 0;
                node.parent = null;
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateHeuristic(startNode, endNode);

            while (openSet.Count > 0)
            {
                Node currentNode = GetLowestFCostNode(openSet);
                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Node neighbor in GetNeighbors(grid, currentNode, gridWidth, gridHeight))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float newGCost = currentNode.gCost + CalculateDistance(currentNode, neighbor);
                    if (newGCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newGCost;
                        neighbor.hCost = CalculateHeuristic(neighbor, endNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get the node from world position.
        /// </summary>
        public static Node GetNodeFromWorldPosition(Node[,] grid, Vector3 worldPosition, int gridWidth, int gridHeight)
        {
            float percentX = (worldPosition.x + gridWidth / 2) / gridWidth;
            float percentY = (worldPosition.y + gridHeight / 2) / gridHeight;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridWidth - 1) * percentX);
            int y = Mathf.RoundToInt((gridHeight - 1) * percentY);
            return grid[x, y];
        }

        /// <summary>
        /// Retrace the path from the end node to the start node.
        /// </summary>
        private static List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Get the node with the lowest F cost.
        /// </summary>
        private static Node GetLowestFCostNode(List<Node> nodes)
        {
            Node lowestFCostNode = nodes[0];
            foreach (Node node in nodes)
            {
                if (node.fCost < lowestFCostNode.fCost || (node.fCost == lowestFCostNode.fCost && node.hCost < lowestFCostNode.hCost))
                {
                    lowestFCostNode = node;
                }
            }
            return lowestFCostNode;
        }

        /// <summary>
        /// Get the neighbors of a given node.
        /// </summary>
        private static List<Node> GetNeighbors(Node[,] grid, Node node, int gridWidth, int gridHeight)
        {
            List<Node> neighbors = new List<Node>();
            int x = node.gridPosition.x;
            int y = node.gridPosition.y;

            if (x - 1 >= 0) neighbors.Add(grid[x - 1, y]);
            if (x + 1 < gridWidth) neighbors.Add(grid[x + 1, y]);
            if (y - 1 >= 0) neighbors.Add(grid[x, y - 1]);
            if (y + 1 < gridHeight) neighbors.Add(grid[x, y + 1]);

            return neighbors;
        }

        /// <summary>
        /// Calculate the distance between two nodes.
        /// </summary>
        private static float CalculateDistance(Node nodeA, Node nodeB)
        {
            float distX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            float distY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);
            return distX + distY; // Manhattan distance
        }

        /// <summary>
        /// Calculate the heuristic between two nodes.
        /// </summary>
        private static float CalculateHeuristic(Node nodeA, Node nodeB)
        {
            return CalculateDistance(nodeA, nodeB);
        }
    }
    public class PathfindingLibrary
{
    /// <summary>
    /// Finds a path using the grid manager from the start position to the end position.
    /// </summary>
    /// <param name="gridManager">The grid manager responsible for pathfinding.</param>
    /// <param name="start">The starting point of the path.</param>
    /// <param name="end">The destination point of the path.</param>
    /// <returns>A list of nodes representing the path.</returns>
    public static List<Node> FindPath(GridManager gridManager, Vector3 start, Vector3 end)
    {
        if (gridManager == null)
        {
            Debug.LogWarning("GridManager is not assigned.");
            return null;
        }

        return gridManager.FindPath(start, end);
    }

    /// <summary>
    /// Moves an object along a given path.
    /// </summary>
    /// <param name="objectToMove">The object that will move along the path.</param>
    /// <param name="path">The path along which the object will move.</param>
    /// <param name="moveSpeed">The speed of the movement.</param>
    /// <param name="onPathCompleted">Callback when the path movement is completed.</param>
    public static IEnumerator MoveAlongPath(Transform objectToMove, List<Node> path, float moveSpeed, System.Action onPathCompleted = null)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("Path is null or empty.");
            yield break;
        }

        foreach (Node node in path)
        {
            Vector3 startPos = objectToMove.position;
            Vector3 endPos = node.worldPosition;
            float journeyLength = Vector3.Distance(startPos, endPos);
            float startTime = Time.time;

            while (Vector3.Distance(objectToMove.position, endPos) > 0.1f)
            {
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distanceCovered / journeyLength;
                objectToMove.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
                Debug.Log($"{startPos} , {endPos}");
                yield return null;
            }
        }

        onPathCompleted?.Invoke(); // Call the callback when done
    }
    public static IEnumerator MoveAlongPathRealTime(Transform objectToMove, List<Node> path, float moveSpeed, Vector3 lastEndPoint, float pathUpdateThreshold, System.Action onPathCompleted = null)
    {

        foreach (Node node in path)
        {
            Vector3 startPos = objectToMove.position;
            Vector3 targetPos = node.worldPosition;
            
            float journeyLength = Vector3.Distance(startPos, targetPos);
            float startTime = Time.time;

            while (Vector3.Distance(objectToMove.position, targetPos) > 0.1f)
            {
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distanceCovered / journeyLength;
                objectToMove.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);

                if (Vector3.Distance(targetPos, lastEndPoint) > pathUpdateThreshold)
                {
                    yield break;
                }

                yield return null;
            }
        }

        Debug.Log("Path completed.");
    }

    /// <summary>
    /// Draws the path in the scene using Gizmos.
    /// </summary>
    /// <param name="path">The path to be drawn.</param>
    /// <param name="color">The color of the path.</param>
    public static void DrawPathGizmos(List<Node> path, Color color)
    {
        if (path == null || path.Count < 2) return;

        Gizmos.color = color;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
        }
    }
}
}
