using System;
using System.Linq;
using Mirror.SimpleWeb;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class FindScriptRecursively: EditorWindow
{
    static int go_count = 0, components_count = 0, missing_count = 0;

    private static string scriptName = String.Empty;

    [MenuItem("Window/FindScriptRecursively")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindScriptRecursively));
    }

    public void OnGUI()
    {
        GUILayout.Label("Script\\component substring to search:");
        scriptName = GUILayout.TextField(scriptName);
        if (GUILayout.Button("Find Script in selected\\all GameObjects no scene"))
        {
            scriptName = scriptName.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(scriptName))
            {
                Debug.LogWarning("Search name is empty!");
                return;
            }
            FindInSelected(scriptName);
        }
    }
    private static void FindInSelected(string scriptName)
    {
        GameObject[] go = Selection.gameObjects;
        if (!go.Any())
        {
            go = GameObject.FindObjectsOfType<GameObject>();
        }

        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g, scriptName);
        }
        Debug.Log(string.Format("Searched '{3}' in {0} GameObjects, {1} components, found {2}", go_count, components_count, missing_count, scriptName));
    }

    private static void FindInGO(GameObject g, string scriptName)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            
            components_count++;
            if (components[i]!= null && components[i].ToString().ToLowerInvariant().Contains(scriptName))
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
                Debug.LogWarning(s + " has an script '"+ components[i].ToString() + "' attached in position: " + i, g);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject, scriptName);
        }
    }
}