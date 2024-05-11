using UnityEditor;
using UnityEngine;
public class RenameCheckpoints : ScriptableObject
{
    [MenuItem("Tools/Rename Checkpoints")]
    static void RenameSelectedCheckpoints()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        //// Sort GameObjects by their current name to keep the order consistent
        //System.Array.Sort(selectedObjects, (a, b) => string.Compare(a.name, b.name));

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            // Renaming the GameObjects to be "Checkpoint" followed by the index
            selectedObjects[i].name = i.ToString();
        }
    }
}