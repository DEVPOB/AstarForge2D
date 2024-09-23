using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public int roomCount = 10;
    public int minRoomSpacing = 5;
    public int maxRoomSpacing = 15;
    public int corridorWidth;

    public Vector2Int roomMinSize = new Vector2Int(4, 4);
    public Vector2Int roomMaxSize = new Vector2Int(8, 8);

    public GameObject roomPrefab;
    public GameObject corridorPrefab;
    public GameObject wallPrefab;
    public GameObject keyItemPrefab;
    public GameObject doorPrefab;

    private TileType[,] grid;
    private enum TileType
    {
        Empty,
        Room,
        Corridor,
        Wall
    }

    public List<Rect> rooms = new List<Rect>();

    void Start()
    {
        //GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        GameObject parent = new GameObject("Parent");

        grid = new TileType[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = TileType.Empty;
            }
        }

        for (int i = 0; i < roomCount; i++)
        {
            CreateRoom(parent);
        }

        for (int i = 1; i < rooms.Count; i++)
        {
            CreateCorridor(rooms[i - 1], rooms[i]);
        }
        PlaceKeyItemInRandomRoom(parent);

        InstantiateDungeon(parent);
    }
    public void ResetGrid()
    {
        DestroyImmediate(GameObject.Find("Parent"));
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = TileType.Empty;
            }
        }

        rooms.Clear();
    }
    void CreateRoom(GameObject Parent)
    {
        int roomWidth = Random.Range(roomMinSize.x, roomMaxSize.x);
        int roomHeight = Random.Range(roomMinSize.y, roomMaxSize.y);

        for (int attempt = 0; attempt < 100; attempt++)
        {
            int roomX = Random.Range(1, gridWidth - roomWidth - 1);
            int roomY = Random.Range(1, gridHeight - roomHeight - 1);

            Rect newRoom = new Rect(roomX, roomY, roomWidth, roomHeight);

            bool validRoomPlacement = true;
            foreach (Rect existingRoom in rooms)
            {
                float distance = CalculateRoomDistance(newRoom, existingRoom);

                if (distance < minRoomSpacing || distance > maxRoomSpacing)
                {
                    validRoomPlacement = false;
                    break;
                }
            }

            if (validRoomPlacement)
            {
                rooms.Add(newRoom);
                for (int x = roomX - 1; x <= roomX + roomWidth; x++)
                {
                    for (int y = roomY - 1; y <= roomY + roomHeight; y++)
                    {
                        if (x == roomX - 1 || x == roomX + roomWidth || y == roomY - 1 || y == roomY + roomHeight)
                        {
                            grid[x, y] = TileType.Wall;
                        }
                        else
                        {
                            grid[x, y] = TileType.Room;
                        }
                    }
                }
                AddRandomDoor(newRoom, Parent);

                return;
            }
        }

    }
    void AddRandomDoor(Rect room, GameObject Parent)
    {
        int roomX = (int)room.xMin;
        int roomY = (int)room.yMin;
        int roomWidth = (int)room.width;
        int roomHeight = (int)room.height;

        int wall = Random.Range(0, 4);
        int doorX = 0, doorY = 0;

        switch (wall)
        {
            case 0: // Top wall
                doorX = Random.Range(roomX + 1, roomX + roomWidth - 1); // Exclude corners
                doorY = roomY + roomHeight;
                break;
            case 1: // Bottom wall
                doorX = Random.Range(roomX + 1, roomX + roomWidth - 1); // Exclude corners
                doorY = roomY - 1;
                break;
            case 2: // Left wall
                doorX = roomX - 1;
                doorY = Random.Range(roomY + 1, roomY + roomHeight - 1); // Exclude corners
                break;
            case 3: // Right wall
                doorX = roomX + roomWidth;
                doorY = Random.Range(roomY + 1, roomY + roomHeight - 1); // Exclude corners
                break;
        }

        if (grid[doorX, doorY] == TileType.Wall)
        {
            grid[doorX, doorY] = TileType.Room;
            Instantiate(doorPrefab, new Vector3(doorX, doorY, 0), Quaternion.identity, Parent.transform); // Instantiate the door in the scene
        }

    }

    void CreateCorridor(Rect roomA, Rect roomB)
    {

        Vector2Int centerA = new Vector2Int((int)roomA.center.x, (int)roomA.center.y);
        Vector2Int centerB = new Vector2Int((int)roomB.center.x, (int)roomB.center.y);

        if (Random.value < 0.5f)
        {
            CreateHorizontalCorridor(centerA.x, centerB.x, centerA.y);
            CreateVerticalCorridor(centerA.y, centerB.y, centerB.x);
        }
        else
        {
            CreateVerticalCorridor(centerA.y, centerB.y, centerA.x);
            CreateHorizontalCorridor(centerA.x, centerB.x, centerB.y);
        }
    }

    void CreateHorizontalCorridor(int x1, int x2, int y)
    {
        int startX = Mathf.Min(x1, x2);
        int endX = Mathf.Max(x1, x2);
        for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x++)
        {
            for (int w = -corridorWidth / 2; w <= corridorWidth / 2; w++)
            {
                int newY = y + w;
                if (newY >= 0 && newY < gridHeight)
                {
                    grid[x, newY] = TileType.Corridor;
                    if (newY - 1 > 0 && grid[x, newY - 1] == TileType.Empty)
                    {
                        if (grid[x, newY - 1] == TileType.Room)
                        {
                            print("In");
                            grid[x, newY - 1] = TileType.Room;
                        }

                        grid[x, newY - 1] = TileType.Wall;
                    }
                    if (newY + 1 < gridHeight && grid[x, newY + 1] == TileType.Empty)
                    {
                        if (grid[x, newY + 1] == TileType.Room)
                        {
                            print("In");
                            grid[x, newY + 1] = TileType.Room;
                        }
                        grid[x, newY + 1] = TileType.Wall;
                    }
                }
            }
            for (int w = -corridorWidth / 2; w <= corridorWidth / 2; w++)
            {
                int wallY1 = y + w;
                int wallY2 = wallY1 - 1;
                int wallY3 = wallY1 + 1;

                if (startX - 1 >= 0 && wallY1 >= 0 && wallY1 < gridHeight && grid[startX - 1, wallY1] == TileType.Empty)
                {
                    grid[startX - 1, wallY1] = TileType.Wall;
                }
                if (startX - 1 >= 0 && wallY2 >= 0 && wallY2 < gridHeight && grid[startX - 1, wallY2] == TileType.Empty)
                {
                    grid[startX - 1, wallY2] = TileType.Wall;
                }
                if (startX - 1 >= 0 && wallY3 >= 0 && wallY3 < gridHeight && grid[startX - 1, wallY3] == TileType.Empty)
                {
                    grid[startX - 1, wallY3] = TileType.Wall;
                }

                if (endX + 1 < gridWidth && wallY1 >= 0 && wallY1 < gridHeight && grid[endX + 1, wallY1] == TileType.Empty)
                {
                    grid[endX + 1, wallY1] = TileType.Wall;
                }
                if (endX + 1 < gridWidth && wallY2 >= 0 && wallY2 < gridHeight && grid[endX + 1, wallY2] == TileType.Empty)
                {
                    grid[endX + 1, wallY2] = TileType.Wall;
                }
                if (endX + 1 < gridWidth && wallY3 >= 0 && wallY3 < gridHeight && grid[endX + 1, wallY3] == TileType.Empty)
                {
                    grid[endX + 1, wallY3] = TileType.Wall;
                }

            }
        }
    }

    void CreateVerticalCorridor(int y1, int y2, int x)
    {
        int startY = Mathf.Min(y1, y2);
        int endY = Mathf.Max(y1, y2);
        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
        {

            for (int w = -corridorWidth / 2; w <= corridorWidth / 2; w++)
            {
                int newX = x + w;
                if (newX >= 0 && newX < gridWidth)
                {
                    grid[newX, y] = TileType.Corridor;

                    if (newX - 1 >= 0 && grid[newX - 1, y] == TileType.Empty)
                    {
                        if (grid[newX, y - 1] == TileType.Room)
                        {
                            grid[newX, y - 1] = TileType.Room;
                        }
                        grid[newX - 1, y] = TileType.Wall;
                    }
                    if (newX + 1 < gridWidth && grid[newX + 1, y] == TileType.Empty)
                    {
                        if (grid[newX, y + 1] == TileType.Room)
                        {
                            grid[newX, y + 1] = TileType.Room;
                        }
                        grid[newX + 1, y] = TileType.Wall;
                    }
                }
            }
            for (int w = -corridorWidth / 2; w <= corridorWidth / 2; w++)
            {
                int wallX1 = x + w;
                int wallX2 = wallX1 - 1;
                int wallX3 = wallX1 + 1;

                if (startY - 1 >= 0 && wallX1 >= 0 && wallX1 < gridWidth && grid[wallX1, startY - 1] == TileType.Empty)
                {
                    grid[wallX1, startY - 1] = TileType.Wall;
                }
                if (startY - 1 >= 0 && wallX2 >= 0 && wallX2 < gridWidth && grid[wallX2, startY - 1] == TileType.Empty)
                {
                    grid[wallX2, startY - 1] = TileType.Wall;
                }
                if (startY - 1 >= 0 && wallX3 >= 0 && wallX3 < gridWidth && grid[wallX3, startY - 1] == TileType.Empty)
                {
                    grid[wallX3, startY - 1] = TileType.Wall;
                }

                if (endY + 1 < gridHeight && wallX1 >= 0 && wallX1 < gridWidth && grid[wallX1, endY + 1] == TileType.Empty)
                {
                    grid[wallX1, endY + 1] = TileType.Wall;
                }
                if (endY + 1 < gridHeight && wallX2 >= 0 && wallX2 < gridWidth && grid[wallX2, endY + 1] == TileType.Empty)
                {
                    grid[wallX2, endY + 1] = TileType.Wall;
                }
                if (endY + 1 < gridHeight && wallX3 >= 0 && wallX3 < gridWidth && grid[wallX3, endY + 1] == TileType.Empty)
                {
                    grid[wallX3, endY + 1] = TileType.Wall;
                }

            }

        }
    }

    float CalculateRoomDistance(Rect roomA, Rect roomB)
    {
        Vector2 centerA = roomA.center;
        Vector2 centerB = roomB.center;

        return Vector2.Distance(centerA, centerB);
    }

    void PlaceKeyItemInRandomRoom(GameObject parent)
    {
        if (rooms.Count == 0) return;

        Rect selectedRoom = rooms[Random.Range(0, rooms.Count)];

        int roomX = (int)Random.Range(selectedRoom.xMin + 1, selectedRoom.xMax - 1); // Exclude walls
        int roomY = (int)Random.Range(selectedRoom.yMin + 1, selectedRoom.yMax - 1); // Exclude walls

        Vector3 keyItemPosition = new Vector3(roomX, roomY, 0);
        Instantiate(keyItemPrefab, keyItemPosition, Quaternion.identity, parent.transform);

        grid[roomX, roomY] = TileType.Room;
    }
    public void SpawnPlayer(GameObject PlayerPref, GameObject parent)
    {
        if (rooms.Count == 0) return; // Ensure there are rooms

        Rect selectedRoom = rooms[Random.Range(0, rooms.Count)];

        int roomX = (int)Random.Range(selectedRoom.xMin + 1, selectedRoom.xMax - 1);
        int roomY = (int)Random.Range(selectedRoom.yMin + 1, selectedRoom.yMax - 1);
        Vector3 localPlayerPos = new Vector3(roomX, roomY, 0);

        GameObject spawnedPlayer = Instantiate(PlayerPref, parent.transform);

        spawnedPlayer.transform.localPosition = localPlayerPos;

    }

    void InstantiateDungeon(GameObject parent)
    {

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x, y, 0);

                if (grid[x, y] == TileType.Room)
                {
                    Instantiate(roomPrefab, position, Quaternion.identity, parent.transform);
                }
                else if (grid[x, y] == TileType.Corridor)
                {
                    Instantiate(corridorPrefab, position, Quaternion.identity, parent.transform);
                }
                else if (grid[x, y] == TileType.Wall)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, parent.transform);
                }
            }
        }
        CenterParent(parent);

    }
    void CenterParent(GameObject parent)
    {
        Vector3 dungeonCenter = new Vector3(gridWidth / 2f, gridHeight / 2f, 0);

        parent.transform.position = -dungeonCenter;

    }

}