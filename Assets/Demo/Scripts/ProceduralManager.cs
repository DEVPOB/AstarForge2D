using UnityEngine;

public class ProceduralManager : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;
    public GridManager gridManager;
    public GameObject Player;
    void Start()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.GenerateDungeon();
            dungeonGenerator.SpawnPlayer(Player, GameObject.Find("Parent"));
            gridManager.enabled = true;
        }
        else
        {
            Debug.LogError("DungeonGenerator is not assigned!");
        }
    }
}
