using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void FindAll()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int found = 0;
        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component c in components)
            {
                if (c == null)
                {
                    Debug.LogWarning("Missing script found on: " + go.name, go);
                    found++;
                }
            }
        }
        if (found == 0)
            Debug.Log("No missing scripts found in scene.");
        else
            Debug.Log("Found " + found + " missing script(s). Check warnings above.");
    }
}
