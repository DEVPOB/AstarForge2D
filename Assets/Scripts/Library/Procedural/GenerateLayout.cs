using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator)),CanEditMultipleObjects]
public class GenerateLayout : Editor
{
    public override void OnInspectorGUI()
    {
        DungeonGenerator generator = (DungeonGenerator)target;
        if (GUILayout.Button("Generate room")){
            generator.GenerateDungeon();
        }
        if (GUILayout.Button("Reset grid")){
            generator.ResetGrid();
        }
        base.OnInspectorGUI();
    }
    
}
